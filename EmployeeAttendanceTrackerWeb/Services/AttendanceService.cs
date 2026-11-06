using EmployeeAttendanceTrackerWeb.Data;
using EmployeeAttendanceTrackerWeb.Helpers;
using EmployeeAttendanceTrackerWeb.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceTrackerWeb.Services;

public class AttendanceService
{
    #region Fields and Constructor

    private readonly AppDbContext _context;
    private readonly WorkingDayHelper _workingDayHelper;

    public AttendanceService(AppDbContext context, WorkingDayHelper workingDayHelper)
    {
        _context = context;
        _workingDayHelper = workingDayHelper;
    }

    #endregion

    #region Today's Attendance

    // Loads today's attendance records.
    public async Task<TodayAttendanceViewModel> GetTodayAttendanceAsync()
    {
        DateTime today = DateTime.Today;
        // DateTime tomorrow = today.AddDays(1);

        var attendances = await (
          from attendance in _context.Attendances.AsNoTracking()
          join employee in _context.Users.AsNoTracking()
              on attendance.UserId equals employee.Id
          where !attendance.IsDeleted
                && !employee.IsDeleted
                && employee.IsActive
                && attendance.CreatedOn.Date == today
          orderby attendance.CreatedOn descending
          select new RecentAttendanceViewModel
          {
              Id = attendance.Id,
              EmployeeName = employee.UserName,
              LoginTime = attendance.CreatedOn,
              IPAddress = attendance.IPAddress,
          })
            .ToListAsync();

        return new TodayAttendanceViewModel
        {
            TotalPresent = attendances
                .Select(a => a.Id)
                .Distinct()
                .Count(),

            FirstLogin = attendances
                .Select(a => (DateTime?)a.LoginTime)
                .Min(),

            LastLogin = attendances
                .Select(a => (DateTime?)a.LoginTime)
                .Max(),

            Attendances = attendances
        };
    }

    #endregion

    #region Attendance Report

    // Loads employee attendance report for the selected date range.
    public async Task<AttendanceReportViewModel> GetAttendanceReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        DateTime today = DateTime.Today;

        // Load all active employees.
        var employees = await _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted && u.IsActive)
            .ToListAsync();

        // Find the earliest joining date among active employees.
        DateTime? firstJoiningDate = employees
            .Select(u => (DateTime?)u.CreatedOn.Date)
            .Min();

        // Set default report dates.
        DateTime reportFromDate = fromDate?.Date
            ?? firstJoiningDate
            ?? today;

        DateTime reportToDate = toDate?.Date ?? today;

        // Prevent calculations beyond today's date.
        DateTime effectiveToDate = reportToDate > today ? today : reportToDate;

        // Load attendance records within the report date range.
        var attendanceRecords = await _context.Attendances
            .AsNoTracking()
            .Where(a => !a.IsDeleted
                        && a.CreatedOn >= reportFromDate
                        && a.CreatedOn < effectiveToDate.AddDays(1))
            .Select(a => new
            {
                a.UserId,
                AttendanceDate = a.CreatedOn.Date
            })
            .ToListAsync();

        // Group attendance dates by employee.
        var attendanceByEmployee = attendanceRecords
            .GroupBy(a => a.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(a => a.AttendanceDate).ToHashSet()
            );

        // Initialize report.
        var report = new AttendanceReportViewModel
        {
            FromDate = fromDate?.Date,
            ToDate = toDate?.Date
        };

        foreach (var employee in employees)
        {
            // Get the employee's actual joining date.
            DateTime joiningDate = employee.CreatedOn.Date;

            attendanceByEmployee.TryGetValue(employee.Id, out var employeeAttendanceDates);

            // Start from the later of the selected From Date and the employee's actual joining date.
            DateTime employeeReportStart =
                reportFromDate > joiningDate ? reportFromDate : joiningDate;

            int totalWorkingDays = 0;
            int presentDays = 0;

            // Calculate present and total working days.
            for (DateTime date = employeeReportStart;
                 date <= effectiveToDate;
                 date = date.AddDays(1))
            {
                if (!_workingDayHelper.IsWorkingDay(date))
                    continue;

                totalWorkingDays++;

                if (employeeAttendanceDates != null && employeeAttendanceDates.Contains(date))
                {
                    presentDays++;
                }
            }

            // Calculate absent days.
            int absentDays = totalWorkingDays - presentDays;

            // // Calculate leave credits from joining month.
            // int leaveCredits = 0;

            // if (effectiveToDate >= joiningDate)
            // {
            //     leaveCredits =
            //         ((effectiveToDate.Year - joiningDate.Year) * 12)
            //         + effectiveToDate.Month
            //         - joiningDate.Month
            //         + 1;
            // }

            // Calculate leave credits.
            int leaveCredits = 0;

            DateTime creditStartDate;

            if (fromDate.HasValue && toDate.HasValue)
            {
                // When filter is applied, start from the later of
                // selected From Date and employee joining date.
                creditStartDate = reportFromDate > joiningDate
                    ? reportFromDate
                    : joiningDate;
            }
            else
            {
                // Without filter, calculate from employee joining month.
                creditStartDate = joiningDate;
            }

            if (creditStartDate <= effectiveToDate)
            {
                leaveCredits =
                    ((effectiveToDate.Year - creditStartDate.Year) * 12)
                    + effectiveToDate.Month
                    - creditStartDate.Month
                    + 1;
            }

            // Add employee report.
            report.Employees.Add(new EmployeeAttendanceReportItemViewModel
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.UserName,
                AbsentDays = absentDays,
                LeaveCredits = leaveCredits,
                PresentDays = presentDays,
                TotalWorkingDays = totalWorkingDays
            });
        }

        return report;
    }

    #endregion

    #region Attendance History

    // Loads attendance history based on employee and date filters.
    public async Task<AttendanceHistoryViewModel> GetAttendanceHistoryAsync(
        AttendanceHistoryFilterViewModel filter)
    {
        // Get today's date.
        DateTime today = DateTime.Today;

        // Set default date range to the current month.
        DateTime fromDate = filter.FromDate?.Date
             ?? new DateTime(today.Year, today.Month, 1);

        DateTime toDate = filter.ToDate?.Date
             ?? today;

        // Load active employees for the dropdown.
        var employees = await _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted && u.IsActive)
            .OrderBy(u => u.UserName)
            .Select(u => new EmployeeDropdownViewModel
            {
                Id = u.Id,
                EmployeeName = u.UserName
            })
            .ToListAsync();

        // Start with attendance records.
        var query =
            from attendance in _context.Attendances.AsNoTracking()
            join employee in _context.Users.AsNoTracking()
                on attendance.UserId equals employee.Id
            where !attendance.IsDeleted
                  && !employee.IsDeleted
                  && employee.IsActive
                  && attendance.CreatedOn >= fromDate
                  && attendance.CreatedOn < toDate.AddDays(1)
            select new { attendance, employee };

        // Get attendance records, newest first.
        var attendances = await query
            .OrderByDescending(x => x.attendance.CreatedOn)
            .Select(x => new AttendanceHistoryItemViewModel
            {
                Id = x.attendance.Id,
                EmployeeName = x.employee.UserName,
                AttendanceDate = x.attendance.CreatedOn.Date,
                LoginTime = x.attendance.CreatedOn,
                IPAddress = x.attendance.IPAddress
            })
            .ToListAsync();

        // Return attendance history.
        return new AttendanceHistoryViewModel
        {
            Filter = new AttendanceHistoryFilterViewModel
            {
                EmployeeId = filter.EmployeeId,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                Employees = employees
            },
            Attendances = attendances
        };
    }

    #endregion
}
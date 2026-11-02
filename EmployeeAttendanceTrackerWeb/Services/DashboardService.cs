using EmployeeAttendanceTrackerWeb.Data;
using EmployeeAttendanceTrackerWeb.Helpers;
using EmployeeAttendanceTrackerWeb.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceTrackerWeb.Services;

// Service for loading dashboard data.
public class DashboardService
{
    #region Feilds and Constructor
    
    private readonly AppDbContext _context;
    private readonly WorkingDayHelper _workingDayHelper;

    public DashboardService(AppDbContext context, WorkingDayHelper workingDayHelper)
    {
        _context = context;
        _workingDayHelper = workingDayHelper;
    }

    #endregion

    #region Dashboard Data

    // Loads all summary information for the dashboard.
    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        DateTime today = DateTime.Today;

        var model = new DashboardViewModel();

        // Check whether today is a working day.
        bool isWorkingDay = _workingDayHelper.IsWorkingDay(today);

        // 1. Load employees.
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.IsActive
            })
            .ToListAsync();

        int activeEmployees = users.Count(u => u.IsActive);

        model.TotalEmployees = users.Count;

        // Get active employee IDs.
        var activeEmployeeIds = users
            .Where(u => u.IsActive)
            .Select(u => u.Id)
            .ToList();

        // 2. Load today's attendance records.
        var todayAttendances = await _context.Attendances
            .AsNoTracking()
            .Where(a =>
                !a.IsDeleted &&
                activeEmployeeIds.Contains(a.UserId) &&
                a.CreatedOn.Date == today)
            .ToListAsync();

        // 3. Calculate today's attendance summary.
        if (isWorkingDay)
        {
            model.PresentToday = todayAttendances
                .Select(a => a.UserId)
                .Distinct()
                .Count();

            model.AbsentToday = Math.Max(0, activeEmployees - model.PresentToday);

        }

        // 4. Prepare today's attendance chart.
        model.TodayChartLabels = new List<string>
        {
            "Present",
            "Absent"
        };

        model.TodayChartData = new List<int>
        {
            model.PresentToday,
            model.AbsentToday
        };

        // 5. Load weekly attendance data.
        await GetWeeklyAttendanceAsync(model, activeEmployeeIds, today);

        // 6. Load recent attendance records.
        model.RecentAttendances = await (
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

        return model;
    }

    #endregion

    #region Weekly Attendance

    // Loads attendance statistics for the current week.
    private async Task GetWeeklyAttendanceAsync(DashboardViewModel model, List<int> activeEmployeeIds,
        DateTime today)
    {
        // Calculate Monday of the current week.
        int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;

        DateTime monday = today.AddDays(-daysSinceMonday);

        // Monday to Friday.
        for (int i = 0; i < 5; i++)
        {
            DateTime date = monday.AddDays(i);

            // Skip weekends and company holidays.
            if (!_workingDayHelper.IsWorkingDay(date))
                continue;

            model.WeeklyLabels.Add(date.ToString("ddd"));

            // Future dates should not have attendance counts.
            if (date > today)
            {
                model.WeeklyPresentData.Add(0);
                model.WeeklyAbsentData.Add(0);
                continue;
            }

            // Count employees who marked attendance on this date.
            int present = await _context.Attendances
                .AsNoTracking()
                .Where(a =>
                    !a.IsDeleted &&
                    activeEmployeeIds.Contains(a.UserId) &&
                    a.CreatedOn.Date == date)
                .Select(a => a.UserId)
                .Distinct()
                .CountAsync();

            // Calculate absent employees.
            int absent = Math.Max(0, activeEmployeeIds.Count - present);

            model.WeeklyPresentData.Add(present);
            model.WeeklyAbsentData.Add(absent);
        }
    }

    #endregion

}
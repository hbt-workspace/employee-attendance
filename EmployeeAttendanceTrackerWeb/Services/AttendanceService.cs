using EmployeeAttendanceTrackerWeb.Data;
using EmployeeAttendanceTrackerWeb.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceTrackerWeb.Services;

public class AttendanceService
{
    #region Fields and Constructor

    private readonly AppDbContext _context;

    public AttendanceService(AppDbContext context)
    {
        _context = context;
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
}
using Microsoft.EntityFrameworkCore;
using EmployeeAttendanceTracker.Models.ViewModels;

namespace EmployeeAttendanceTracker.Services;

// AttendanceService class for loading dashboard data.
public class AttendanceService
{
    #region  Methods for loading dashboard data

    public async Task<DashboardDataViewModel> GetDashboardDataAsync()
    {
        await using var db = AppDbContextFactory.CreateDbContext();

        DateTime currentDateTime = DateTime.Now;

        // 1. Load only the employee fields required by the UI.
        var users = await db.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.IsActive
            })
            .ToListAsync();

        var employees = users.Select(u => new EmployeeRowViewModel
        {
            Id = u.Id,
            UserName = u.UserName,
            Status = u.IsActive ? "Active" : "Inactive"
        }).ToList();

        for (int i = 0; i < employees.Count; i++)
        {
            employees[i].SerialNumber  = i + 1;
        }

        int activeEmployees = users.Count(u => u.IsActive);

        // 2. Load most recent attendance records.
        var records = await (
            from a in db.Attendances.AsNoTracking()
            join u in db.Users.AsNoTracking()
                on a.UserId equals u.Id
            where !a.IsDeleted && !u.IsDeleted && a.CreatedOn.Date == currentDateTime.Date
            orderby a.CreatedOn descending
            select new AttendanceRowViewModel
            {
                Id = a.Id,
                EmployeeId = u.Id,
                EmployeeName = u.UserName,
                AttendanceDate = a.CreatedOn,
                LoginTime = a.CreatedOn,
                IPAddress = a.IPAddress
            })
            .ToListAsync();

        // 3. Count today's attendance directly in Db. Only active, non-deleted employees are counted as present.
        int presentToday = await (
            from a in db.Attendances.AsNoTracking()
            join u in db.Users.AsNoTracking()
                on a.UserId equals u.Id
            where !a.IsDeleted
                  && !u.IsDeleted
                  && u.IsActive
                  && a.CreatedOn.Date == currentDateTime.Date
            select a.UserId
        )
        .Distinct()
        .CountAsync();

        for (int i = 0; i < records.Count; i++)
        {
            records[i].SerialNumber = i + 1;
        }

        // 4. Prepare the latest-login summary.
        var latest = records.FirstOrDefault();

        // 5. Return all dashboard data.
        return new DashboardDataViewModel
        {
            Employees = employees,
            AttendanceRecords = records,
            TotalEmployees = users.Count,
            PresentToday = presentToday,
            AbsentToday = Math.Max(0, activeEmployees - presentToday),
            LatestLogin = latest?.LoginTime.ToString("hh:mm tt") ?? "--",
            LatestEmployee = latest?.EmployeeName ?? "No attendance records"
        };
    }

    #endregion

    #region  Methods for loading Attendance history data

    // Load all attendance records for the History page.
    public async Task<List<AttendanceRowViewModel>> GetAllAttendanceRecordsAsync()
    {
        await using var db = AppDbContextFactory.CreateDbContext();

        var records = await (
            from a in db.Attendances.AsNoTracking()
            join u in db.Users.AsNoTracking()
                on a.UserId equals u.Id
            where !a.IsDeleted && !u.IsDeleted
            orderby a.CreatedOn descending
            select new AttendanceRowViewModel
            {
                Id = a.Id,
                EmployeeId = u.Id,
                EmployeeName = u.UserName,
                AttendanceDate = a.CreatedOn,
                LoginTime = a.CreatedOn,
                IPAddress = a.IPAddress
            })
            .ToListAsync();

        for (int i = 0; i < records.Count; i++)
        {
            records[i].SerialNumber = i + 1;
        }

        return records;
    }

    #endregion
}
using EmployeeAttendanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceTrackerWeb.Data;

// Database context for the admin panel.

public class AppDbContext : DbContext
{
    #region Constructor

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    #endregion

    #region Db Sets

    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<Attendance> Attendances => Set<Attendance>();

    #endregion   
}

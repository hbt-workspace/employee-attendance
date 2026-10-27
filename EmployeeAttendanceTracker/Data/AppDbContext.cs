using EmployeeAttendanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceTracker.Data;

// Database Context for the application to interact with the database.
public class AppDbContext : DbContext
{
    #region Constructor
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    #endregion

    #region DbSets

    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<Attendance> Attendances => Set<Attendance>();

    #endregion

}
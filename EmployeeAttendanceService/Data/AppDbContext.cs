using EmployeeAttendanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceService.Data;

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

    public DbSet<UserModel> Users { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    #endregion
}
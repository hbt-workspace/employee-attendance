using EmployeeAttendanceTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeAttendanceTracker;

// Database Context Factory for the application to create the database context. 
public static class AppDbContextFactory
{
    #region DbContext Configuration

    private static readonly IConfiguration Configuration =
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json",optional: false,reloadOnChange: false)
            .Build();

    public static AppDbContext CreateDbContext()
    {
        string? connectionString =Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'DefaultConnection' was not found.");
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    #endregion
}

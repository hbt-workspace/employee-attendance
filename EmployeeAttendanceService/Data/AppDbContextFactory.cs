using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EmployeeAttendanceService.Data;

// Factory used by Entity Framework Core tools to create AppDbContext during migrations.
public class AppDbContextFactory: IDesignTimeDbContextFactory<AppDbContext>
{
    #region DbContext Configuration

    public AppDbContext CreateDbContext(string[] args)
    {
        // Load the database configuration.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json",optional: false,reloadOnChange: false)
            .Build();

        // Retrieve the default connection string.
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection is missing from appsettings.json.");
        }

        // Configure Entity Framework Core for SQL Server.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        // Return the configured database context.
        return new AppDbContext(options);
    }

    #endregion
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmployeeAttendanceService.Data;

// RuntimeDbContextFactory class for creating a runtime database context.
public static class RuntimeDbContextFactory
{
    #region Runtime DbContext Configuration

    // Create a configured AppDbContext for application runtime.
    public static AppDbContext CreateDbContext()
    {
        // Resolve the directory containing the application files.
        string applicationDirectory = AppContext.BaseDirectory;

        // Load the application configuration.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(applicationDirectory)
            .AddJsonFile("appsettings.json",optional: false,reloadOnChange: false)
            .Build();

        // Retrieve the SQL Server connection string.
        string? connectionString =
            configuration.GetConnectionString("DefaultConnection");

        // Validate the connection string.
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection is missing from appsettings.json.");
        }

        // Configure Entity Framework Core.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    #endregion
}
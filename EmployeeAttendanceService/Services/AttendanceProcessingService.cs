using EmployeeAttendanceService.Data;
using EmployeeAttendanceService.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceService.Services;

// Attendance Processing Service for the application.
public class AttendanceProcessingService
{
    #region Methods of Attendance Processing Service

    // Execute the attendance workflow.
    public async Task RunAsync()
    {
        try
        {
            // Read the current Windows username and computer name.
            string windowsUserName = Environment.UserName;
            string computerName = Environment.MachineName;

            // Create the database context.
            using var dbContext = RuntimeDbContextFactory.CreateDbContext();

            // Verify database connectivity.
            bool canConnect = await dbContext.Database.CanConnectAsync();

            if (!canConnect)
            {
                throw new InvalidOperationException("Unable to connect to the attendance database.");
            }

            // Find the active employee matching the Windows username.
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == windowsUserName &&
                    u.IsActive &&
                    !u.IsDeleted);

            // Stop if the employee is not registered or active.
            if (user == null)
            {
                throw new InvalidOperationException($"No active employee is registered for Windows user " + $"'{windowsUserName}'.");
            }

            // Get the current local date and time.
            DateTime currentDateTime = DateTime.Now;
            DateTime startOfDay = currentDateTime.Date;
            DateTime startOfNextDay = startOfDay.AddDays(1);

            // Check whether attendance already exists today.
            bool attendanceExists = await dbContext.Attendances
                .AnyAsync(a => a.UserId == user.Id && !a.IsDeleted &&
                    a.CreatedOn >= startOfDay &&
                    a.CreatedOn < startOfNextDay);

            if (attendanceExists)
            {
                FileLoggerHelper.LogInformation($"Attendance already exists for user {user.UserName} " +
                    $"on {startOfDay:yyyy-MM-dd}.");
                return;
            }

            // Create the attendance record using the existing model.
            var attendance = new Models.Attendance
            {
                UserId = user.Id,
                IPAddress = await IPAddressHelper.GetPublicIPAddressAsync(),
                CreatedBy = user.Id,
                CreatedOn = currentDateTime,
                IsDeleted = false,
                UpdatedBy = null,
                UpdatedOn = null
            };

            // Save the attendance record.
            dbContext.Attendances.Add(attendance);
            await dbContext.SaveChangesAsync();

            // Log successful attendance creation.
            FileLoggerHelper.LogInformation($"Attendance marked for user {user.UserName} " +
                $"({computerName}), on {startOfDay:yyyy-MM-dd}.");

        }
        catch (Exception ex)
        {
            // Log failures because the application exits automatically.
            FileLoggerHelper.LogError(ex.Message);
        }
    }

    #endregion    
}
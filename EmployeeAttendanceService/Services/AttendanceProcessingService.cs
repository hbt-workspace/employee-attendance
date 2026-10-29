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
            // Get the current local date and time.
            DateTime currentDateTime = DateTime.Now;

            // Stop attendance processing on weekends and holidays.
            if (!IsWorkingDay(currentDateTime.Date))
            {
                FileLoggerHelper.LogInformation(
                    $"Attendance not marked because {currentDateTime:yyyy-MM-dd} " +
                    "is a weekend or public holiday.");

                return;
            }

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

            // Get or create the current Windows user.
            var userService = new UserService();
            var user = await userService.GetOrCreateUserAsync();

            // Stop if the user account is inactive.
            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    $"The account for '{user.UserName}' is inactive.");
            }

            // Get the current local date and time.
            // DateTime currentDateTime = DateTime.Now;
            //DateTime startOfDay = currentDateTime.Date;
            //DateTime startOfNextDay = startOfDay.AddDays(1);

            // Check whether attendance already exists today.
            bool attendanceExists = await dbContext.Attendances
                .AnyAsync(a => a.UserId == user.Id && !a.IsDeleted &&
                    a.CreatedOn.Date == currentDateTime.Date);

            if (attendanceExists)
            {
                FileLoggerHelper.LogInformation($"Attendance already exists for user {user.UserName} " +
                    $"on {currentDateTime:yyyy-MM-dd}.");
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
                $"({computerName}), on {currentDateTime:yyyy-MM-dd}.");

        }
        catch (Exception ex)
        {
            // Log failures because the application exits automatically.
            FileLoggerHelper.LogError(ex.Message);
        }
    }

    #endregion

    #region Methods for Working Day and Holiday Validation

    // Checks whether a date is a working day.
    private static bool IsWorkingDay(DateTime date)
    {
        // Exclude Saturday and Sunday.
        if (date.DayOfWeek == DayOfWeek.Saturday ||
            date.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }

        // Load holiday dates from appsettings.json.
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json",optional: false,reloadOnChange: false)
            .Build();

        var holidays = configuration
            .GetSection("Holidays")
            .GetChildren()
            .Select(section => section.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value));

        // Exclude configured public holidays.
        string dateString = date.ToString("yyyy-MM-dd");

        return !holidays.Contains(dateString, StringComparer.Ordinal);
    }

    #endregion
}
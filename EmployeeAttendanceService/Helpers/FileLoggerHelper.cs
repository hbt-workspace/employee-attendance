namespace EmployeeAttendanceService.Helpers;

// FileLogger class for logging to a file.
public static class FileLoggerHelper
{
    #region Fields

    // Store the log file beside the application.
    private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "AttendanceService.log");

    #endregion

    #region Public Methods

    // Write an informational message to the log file.
    public static void LogInformation(string message)
    {
        WriteLog("INFO", message);
    }

    // Write an error message to the log file.
    public static void LogError(string message)
    {
        WriteLog("ERROR", message);
    }

    #endregion

    #region Private Methods

    // Append a timestamped message to the log file.
    private static void WriteLog(string level, string message)
    {
        string logMessage =
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} " +
            $"[{level}] {message}{Environment.NewLine}";

        File.AppendAllText(LogFilePath, logMessage);
    }

    #endregion
}

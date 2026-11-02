using System.Globalization;

namespace EmployeeAttendanceTrackerWeb.Helpers;

// Helper for checking working days.
public class WorkingDayHelper
{
    private readonly HashSet<DateTime> _holidays;

    #region Constructor

    public WorkingDayHelper(IConfiguration configuration)
    {
        _holidays = configuration
            .GetSection("Holidays")
            .Get<string[]>()?
            .Select(date => DateTime.ParseExact(date,"yyyy-MM-dd",CultureInfo.InvariantCulture))
            .Select(date => date.Date)
            .ToHashSet() ?? new HashSet<DateTime>();
    }

    #endregion

    #region Methods for Working Day 

    public bool IsWorkingDay(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday
            && date.DayOfWeek != DayOfWeek.Sunday
            && !_holidays.Contains(date.Date);
    }

    #endregion
}
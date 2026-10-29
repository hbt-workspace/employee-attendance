using System.Globalization;
using EmployeeAttendanceTracker.Models.ViewModels;
using Microsoft.Extensions.Configuration;

namespace EmployeeAttendanceTracker.Services;

// FilteringService class for filtering attendance records.
public class FilteringService
{
    private readonly HashSet<DateTime> _holidays;


    // Loads holiday dates from appsettings.json.
    public FilteringService()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _holidays = configuration
        .GetSection("Holidays")
        .GetChildren()
        .Select(section => DateTime.ParseExact(section.Value!,"yyyy-MM-dd",CultureInfo.InvariantCulture))
        .Select(date => date.Date)
        .ToHashSet();
    }

    #region  Methods for filtering attendance records

    // Filter attendance records for the Dashboard page.
    public bool FilterAttendance(AttendanceRowViewModel row, string searchText, int dateFilterIndex)
    {
        searchText = searchText.Trim();

        bool matchesSearch =
            string.IsNullOrWhiteSpace(searchText) ||
            row.EmployeeName.Contains(
                searchText, StringComparison.OrdinalIgnoreCase) ||
            row.IPAddress.Contains(
                searchText, StringComparison.OrdinalIgnoreCase) ||
            row.EmployeeId.ToString().Contains(
                searchText, StringComparison.OrdinalIgnoreCase);

        if (!matchesSearch)
            return false;

        DateTime today = DateTime.Today;

        return dateFilterIndex switch
        {
            1 => row.AttendanceDate.Date == today,
            2 => row.AttendanceDate.Date >= today.AddDays(-6)
                 && row.AttendanceDate.Date <= today,
            3 => row.AttendanceDate.Date >= today.AddDays(-29)
                 && row.AttendanceDate.Date <= today,
            _ => true
        };
    }

    #endregion

    #region  Methods for filtering employee records

    // Filter employee records for the Employees page.
    public bool FilterEmployee(EmployeeRowViewModel employee, string searchText)
    {
        searchText = searchText.Trim();

        return string.IsNullOrWhiteSpace(searchText) ||
               employee.UserName.Contains(
                   searchText, StringComparison.OrdinalIgnoreCase) ||
               employee.Id.ToString().Contains(
                   searchText, StringComparison.OrdinalIgnoreCase) ||
               employee.Status.Contains(
                   searchText, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region  Methods for filtering attendance by Days

    // Checks whether a date is a working day.
    public bool IsWorkingDay(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday
            && date.DayOfWeek != DayOfWeek.Sunday
            && !_holidays.Contains(date.Date);
    }

    // Filter attendace records according to period.
    public bool FilterAttendanceByPeriod(AttendanceRowViewModel row, int selectedIndex)
    {
        DateTime today = DateTime.Today;
        DateTime attendanceDate = row.AttendanceDate.Date;

        // Exclude weekends and public holidays.
        if (!IsWorkingDay(attendanceDate))
        {
            return false;
        }

        return true;

        // switch (selectedIndex)
        // {
        //     case 0: // All days
        //         return true;

        //     case 1: // This week: Monday to Friday, up to today
        //         {
        //             int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;

        //             DateTime monday = today.AddDays(-daysSinceMonday);

        //             DateTime friday = monday.AddDays(4);
        //             DateTime endDate = today < friday ? today : friday;

        //             return row.AttendanceDate.Date >= monday
        //                 && row.AttendanceDate.Date <= endDate;
        //         }

        //     case 2: // This month
        //         {
        //             DateTime firstDay = new DateTime(today.Year, today.Month, 1);

        //             DateTime firstDayNextMonth = firstDay.AddMonths(1);

        //             return row.AttendanceDate.Date >= firstDay
        //                 && row.AttendanceDate.Date < firstDayNextMonth;
        //         }

        //     case 3: // Last month
        //         {
        //             DateTime firstDayThisMonth = new DateTime(today.Year, today.Month, 1);

        //             DateTime firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

        //             return row.AttendanceDate.Date >= firstDayLastMonth
        //                 && row.AttendanceDate.Date < firstDayThisMonth;
        //         }

        //     default:
        //         return true;
        // }
    }

    #endregion
}

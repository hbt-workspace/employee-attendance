using EmployeeAttendanceTracker.Models.ViewModels;

namespace EmployeeAttendanceTracker.Services;

// FilteringService class for filtering attendance records.
public class FilteringService
{
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

    // Filter attendace records according to period.
    public bool FilterAttendanceByPeriod(AttendanceRowViewModel row, int selectedIndex)
    {
        DateTime today = DateTime.Today;

        switch (selectedIndex)
        {
            case 0: // All days
                return true;

            case 1: // This week: Monday to Friday, up to today
                {
                    int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;

                    DateTime monday = today.AddDays(-daysSinceMonday);

                    DateTime friday = monday.AddDays(4);
                    DateTime endDate = today < friday ? today : friday;

                    return row.AttendanceDate.Date >= monday
                        && row.AttendanceDate.Date <= endDate;
                }

            case 2: // This month
                {
                    DateTime firstDay = new DateTime(today.Year, today.Month, 1);

                    DateTime firstDayNextMonth = firstDay.AddMonths(1);

                    return row.AttendanceDate.Date >= firstDay
                        && row.AttendanceDate.Date < firstDayNextMonth;
                }

            case 3: // Last month
                {
                    DateTime firstDayThisMonth = new DateTime(today.Year, today.Month, 1);

                    DateTime firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

                    return row.AttendanceDate.Date >= firstDayLastMonth
                        && row.AttendanceDate.Date < firstDayThisMonth;
                }

            default:
                return true;
        }
    }

    #endregion
}

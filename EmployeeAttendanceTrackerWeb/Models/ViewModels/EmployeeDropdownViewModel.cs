namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// ViewModel for employee dropdown in attendance history filter.
public class EmployeeDropdownViewModel
{
    #region  ViewModel Properties

    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    #endregion
}

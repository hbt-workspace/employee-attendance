namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// ViewModel for attendance history filter.
public class AttendanceHistoryFilterViewModel
{
    #region ViewModel Properties

    public int? EmployeeId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<EmployeeDropdownViewModel> Employees { get; set; } = new();

    #endregion
}
namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// ViewModel for attendance history page.
public class AttendanceHistoryViewModel
{
    #region ViewModel Properties
    
    public AttendanceHistoryFilterViewModel Filter { get; set; } = new();
    public List<AttendanceHistoryItemViewModel> Attendances { get; set; } = new();

    #endregion
}

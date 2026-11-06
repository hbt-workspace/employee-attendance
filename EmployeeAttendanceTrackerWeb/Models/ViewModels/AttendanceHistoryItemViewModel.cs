namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// ViewModel for individual attendance history item.
public class AttendanceHistoryItemViewModel
{
    #region ViewModel Properties

    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }
    public DateTime LoginTime { get; set; }
    public string? IPAddress { get; set; }

    #endregion
}

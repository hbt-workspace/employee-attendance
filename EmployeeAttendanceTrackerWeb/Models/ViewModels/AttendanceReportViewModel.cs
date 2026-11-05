namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// AttendanceReportViewModel represents the data structure for the attendance report view.
public class AttendanceReportViewModel
{
    #region View Model Properties

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<EmployeeAttendanceReportItemViewModel> Employees { get; set; } = new();

    #endregion
}


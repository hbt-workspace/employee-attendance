namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// EmployeeAttendanceReportItemViewModel represents the data structure for an individual employee's attendance report item.
public class EmployeeAttendanceReportItemViewModel
{
    #region View Model Properties

    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int AbsentDays { get; set; }
    public int LeaveCredits { get; set; }
    public int PresentDays { get; set; }
    public int TotalWorkingDays { get; set; }

    #endregion
}
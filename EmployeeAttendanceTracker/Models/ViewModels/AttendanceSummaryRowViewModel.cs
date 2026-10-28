namespace EmployeeAttendanceTracker.Models.ViewModels;

// Attendance Summary Row View Model for the application.
public class AttendanceSummaryRowViewModel
{
    #region View-Model Properties

    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int WorkingDays { get; set; }
    public string IPAddress { get; set; } = string.Empty;

    #endregion
}

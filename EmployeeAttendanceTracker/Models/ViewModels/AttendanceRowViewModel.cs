namespace EmployeeAttendanceTracker.Models.ViewModels;

// Attendance Row View Model for the application.
public class AttendanceRowViewModel 
{
    #region View-Model Properties

    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = "";
    public DateTime AttendanceDate { get; set; }
    public DateTime LoginTime { get; set; }
    public string IPAddress { get; set; } = "";
    public int SerialNumber { get; set; }

    #endregion
}
namespace EmployeeAttendanceTracker.Models;

// Attendance Model for the application.
public class Attendance : BaseModel
{
    #region Data-Model Properties

    public int Id { get; set; }
    public int UserId { get; set; }
    public string IPAddress { get; set; } = string.Empty;

    #endregion
}
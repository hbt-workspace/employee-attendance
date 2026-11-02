namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// Holds individual attendance records for the recent attendance table.
public class RecentAttendanceViewModel
{
    #region  View Model Properties
    
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public string IPAddress { get; set; } = string.Empty;

    #endregion
}
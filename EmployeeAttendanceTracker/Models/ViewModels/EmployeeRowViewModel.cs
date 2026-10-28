namespace EmployeeAttendanceTracker.Models.ViewModels;

// Employee Row View Model for the application.
public class EmployeeRowViewModel
{
    #region View-Model Properties

    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string Status { get; set; } = "";
    public int SerialNumber { get; set; }

    #endregion
}
namespace EmployeeAttendanceTrackerWeb.Models;

public class ErrorViewModel
{
    #region Error Properties
    
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    #endregion
}

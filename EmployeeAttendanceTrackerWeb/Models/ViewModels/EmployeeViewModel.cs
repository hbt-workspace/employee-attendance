namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// ViewModel for representing employee data
public class EmployeeViewModel
{
    #region ViewModel Properties

    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    #endregion
}
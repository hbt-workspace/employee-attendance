namespace EmployeeAttendanceTracker.Models.ViewModels;

// Dashboard Data View Model for the application.
public class DashboardDataViewModel
{
    #region View-Model Properties

    public int TotalEmployees { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public string LatestLogin { get; set; } = "--";
    public string LatestEmployee { get; set; } = "No attendance records";
    public List<EmployeeRowViewModel> Employees { get; set; } = [];
    public List<AttendanceRowViewModel> AttendanceRecords { get; set; } = [];
   
    #endregion
}
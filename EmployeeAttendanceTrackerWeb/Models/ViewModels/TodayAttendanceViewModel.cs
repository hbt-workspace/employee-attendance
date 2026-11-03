namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// ViewModel for today's attendance page.
public class TodayAttendanceViewModel
{
    #region View Model Properties

    public int TotalPresent { get; set; }
    public DateTime? FirstLogin { get; set; }
    public DateTime? LastLogin { get; set; }
    public List<RecentAttendanceViewModel> Attendances { get; set; } = new();

    #endregion
}
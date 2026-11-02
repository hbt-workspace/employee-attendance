namespace EmployeeAttendanceTrackerWeb.Models.ViewModels;

// Holds all data required by the dashboard.
public class DashboardViewModel
{
    #region View Model Properties

    public int TotalEmployees { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public List<string> TodayChartLabels { get; set; } = new();
    public List<int> TodayChartData { get; set; } = new();
    public List<string> WeeklyLabels { get; set; } = new();
    public List<int> WeeklyPresentData { get; set; } = new();
    public List<int> WeeklyAbsentData { get; set; } = new();
    public List<RecentAttendanceViewModel> RecentAttendances { get; set; } = new();

    #endregion
}
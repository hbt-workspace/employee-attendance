using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using EmployeeAttendanceTracker.Helpers;
using EmployeeAttendanceTracker.Models.ViewModels;
using EmployeeAttendanceTracker.Services;

namespace EmployeeAttendanceTracker;

public partial class MainWindow : Window
{
    #region Fields 

    private readonly ObservableCollection<AttendanceRowViewModel> _attendance = new();
    private readonly ObservableCollection<EmployeeRowViewModel> _employees = new();
    private ICollectionView _attendanceView = null!;
    private ICollectionView _employeesView = null!;
    private string _currentPage = "Dashboard";

    private readonly WindowHelper _windowHelper = new();
    private readonly AttendanceService _attendanceService = new();
    private readonly FilteringService _filteringService = new();
    private readonly ObservableCollection<AttendanceRowViewModel> _historyAttendance = new();
    private ICollectionView _historyView = null!;

    private readonly ObservableCollection<AttendanceSummaryRowViewModel> _attendanceReport = new();
    private readonly List<AttendanceRowViewModel> _allReportRecords = new();
    private ICollectionView _attendanceReportView = null!;

    #endregion

    #region Constructor

    // Constructor 
    public MainWindow()
    {
        InitializeComponent();
        _windowHelper.Attach(this);

        TodayText.Text = DateTime.Today.ToString("dddd, dd MMM yyyy");

        _attendanceView = CollectionViewSource.GetDefaultView(_attendance);
        _employeesView = CollectionViewSource.GetDefaultView(_employees);
        _historyView = CollectionViewSource.GetDefaultView(_historyAttendance);

        _attendanceReportView = CollectionViewSource.GetDefaultView(_attendanceReport);
        _attendanceReportView.Filter = FilterAttendanceReport;
        AttendanceReportGrid.ItemsSource = _attendanceReportView;

        _attendanceView.Filter = FilterDashboardAttendance;
        _employeesView.Filter = FilterEmployees;
        _historyView.Filter = FilterHistoryAttendance;

        AttendanceGrid.ItemsSource = _attendanceView;
        HistoryGrid.ItemsSource = _historyView;
        EmployeesGrid.ItemsSource = _employeesView;

        Loaded += async (_, _) => await LoadDataAsync();

    }

    #endregion

    #region Window Button Events

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        _windowHelper.Minimize(this);
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        _windowHelper.ToggleMaximize(this, MaximizeButton);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _windowHelper.Close(this);
    }

    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _windowHelper.HandleHeaderMouseLeftButtonDown(this, MaximizeButton, e);
    }

    #endregion

    #region Data Loading Methods
    
    private async Task LoadDataAsync()
    {
        try
        {
            RefreshButton.IsEnabled = false;
            LoadingText.Text = "Loading attendance data...";

            var dashboardData = await _attendanceService.GetDashboardDataAsync();

            _employees.Clear();

            foreach (var employee in dashboardData.Employees)
            {
                _employees.Add(employee);
            }

            _attendance.Clear();

            foreach (var record in dashboardData.AttendanceRecords)
            {
                _attendance.Add(record);
            }

            await LoadHistoryAsync();

            var reportRecords = await _attendanceService.GetAllAttendanceRecordsAsync();

            _allReportRecords.Clear();
            _allReportRecords.AddRange(reportRecords);

            UpdateAttendanceReport();

            TotalEmployeesText.Text = dashboardData.TotalEmployees.ToString();
            PresentTodayText.Text = dashboardData.PresentToday.ToString();
            AbsentTodayText.Text = dashboardData.AbsentToday.ToString();
            LatestLoginText.Text = dashboardData.LatestLogin;
            LatestEmployeeText.Text = dashboardData.LatestEmployee;
            AttendanceCountText.Text = $"{dashboardData.AttendanceRecords.Count} attendance records today";
            LoadingText.Text = string.Empty;

            _attendanceView.Refresh();
            _employeesView.Refresh();
        }
        catch (Exception ex)
        {
            LoadingText.Text = "Could not load database records.";

            MessageBox.Show(
                this,
                $"Unable to load attendance data.\n\n{ex.Message}",
                "Database Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            RefreshButton.IsEnabled = true;
        }
    }

    // Load all attendance records for the History page.
    private async Task LoadHistoryAsync()
    {
        var records = await _attendanceService.GetAllAttendanceRecordsAsync();
        _historyAttendance.Clear();

        foreach (var record in records)
        {
            _historyAttendance.Add(record);
        }

        _historyView.Refresh();
    }

    #endregion

    #region Filtering Methods

    // Filter attendance records for the Dashboard page.
    private bool FilterDashboardAttendance(object obj)
    {
        if (obj is not AttendanceRowViewModel row)
            return false;

        return _filteringService.FilterAttendance(row, SearchTextBox.Text, 0);
    }

    // Filter attendance records for the History page.
    private bool FilterHistoryAttendance(object obj)
    {
        if (obj is not AttendanceRowViewModel row)
            return false;

        return _filteringService.FilterAttendance(row, SearchTextBox.Text, DateFilter.SelectedIndex);
    }

    // Filter employee records for the Employees page.
    private bool FilterEmployees(object obj)
    {
        if (obj is not EmployeeRowViewModel employee)
            return false;

        return _filteringService.FilterEmployee(employee, SearchTextBox.Text);
    }

    private bool FilterAttendanceReport(object obj)
    {
        if (obj is not AttendanceSummaryRowViewModel row)
            return false;

        string searchText = SearchTextBox.Text.Trim();

        return string.IsNullOrWhiteSpace(searchText) ||
               row.EmployeeName.Contains(
                   searchText, StringComparison.OrdinalIgnoreCase) ||
               row.IPAddress.Contains(
                   searchText, StringComparison.OrdinalIgnoreCase) ||
               row.EmployeeId.ToString().Contains(
                   searchText, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region UI Event Handlers
    
    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync();
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_currentPage == "Employees")
        {
            _employeesView?.Refresh();
        }
        else if (_currentPage == "AttendanceReport")
        {
            _attendanceReportView?.Refresh();
        }
        else if (_currentPage == "Dashboard")
        {
            _attendanceView?.Refresh();
        }
    }

    private void DateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _historyView?.Refresh();
    }

    private void AttendanceReportDateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateAttendanceReport();
    }

    #endregion

    #region Navigation

    private void NavigationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not string page)
        {
            return;
        }

        _currentPage = page;

        DashboardPanel.Visibility =
            page == "Dashboard" ? Visibility.Visible : Visibility.Collapsed;

        EmployeesPanel.Visibility =
            page == "Employees" ? Visibility.Visible : Visibility.Collapsed;

        AttendancePanel.Visibility =
            page == "Attendance" ? Visibility.Visible : Visibility.Collapsed;

        AttendanceReportPanel.Visibility =
            page == "AttendanceReport" ? Visibility.Visible : Visibility.Collapsed;

        PageTitle.Text = page switch
        {
            "Dashboard" => "Dashboard",
            "Employees" => "Employees",
            "AttendanceReport" => "Attendance",
            _ => "Attendance History"
        };

        PageSubtitle.Text = page switch
        {
            "Dashboard" => "Your team's attendance at a glance.",
            "Employees" => "View employee records and account status.",
            "AttendanceReport" => "Employee attendance overview.",
            _ => "Review employee check-ins and recorded IP addresses."
        };

        SearchTextBox.Text = string.Empty;

        SearchTextBox.Visibility = Visibility.Visible;

        DateFilter.Visibility =
            page == "Attendance" ? Visibility.Visible : Visibility.Collapsed;

        UpdateNavigationHighlight(page);

        if (page == "Employees")
        {
            _employeesView.Refresh();
        }
        else if (page == "Attendance")
        {
            _historyView.Refresh();
        }
        else if (page == "AttendanceReport")
        {
            UpdateAttendanceReport();
        }
        else
        {
            _attendanceView.Refresh();
        }
    }

    private void UpdateNavigationHighlight(string activePage)
    {
        Button[] navigationButtons =
        {
        DashboardNavButton,
        AttendanceReportNavButton,
        EmployeesNavButton,
        AttendanceNavButton
    };

        foreach (Button navButton in navigationButtons)
        {
            bool isActive = navButton.Tag?.ToString() == activePage;

            navButton.Background = isActive
                ? new SolidColorBrush(ColorFromHex("#2864E8"))
                : Brushes.Transparent;

            navButton.Foreground = isActive
                ? Brushes.White
                : new SolidColorBrush(ColorFromHex("#C7D3E5"));
        }
    }

    #endregion

    #region Helper Methods

    // Converts a hexadecimal color string into a WPF Color value.
    private static Color ColorFromHex(string hex)
    {
        return (Color)ColorConverter.ConvertFromString(hex);
    }

    // Updates the attendance report based on the selected date filter.
    private void UpdateAttendanceReport()
    {
        if (_attendanceReportView == null ||
            AttendanceReportDateFilter.SelectedIndex < 0)
        {
            return;
        }

        DateTime today = DateTime.Today;
        int selectedIndex = AttendanceReportDateFilter.SelectedIndex;

        DateTime startDate;
        DateTime endDate;

        switch (selectedIndex)
        {
            case 1: // This week: Monday through today
                {
                    int daysSinceMonday =
                        ((int)today.DayOfWeek + 6) % 7;

                    startDate = today.AddDays(-daysSinceMonday);
                    endDate = today;
                    break;
                }

            case 2: // This month through today
                startDate = new DateTime(today.Year, today.Month, 1);
                endDate = today;
                break;

            case 3: // Previous calendar month
                {
                    DateTime firstDayThisMonth =
                        new DateTime(today.Year, today.Month, 1);

                    startDate = firstDayThisMonth.AddMonths(-1);
                    endDate = firstDayThisMonth.AddDays(-1);
                    break;
                }

            default: // All recorded history through today
                startDate = _allReportRecords.Count > 0
                    ? _allReportRecords.Min(r => r.AttendanceDate.Date)
                    : today;

                endDate = today;
                break;
        }

        int workingDays = CountWeekdays(startDate, endDate);

        // Include only weekdays in the selected date range.
        var periodRecords = _allReportRecords
            .Where(r =>
                r.AttendanceDate.Date >= startDate &&
                r.AttendanceDate.Date <= endDate &&
                r.AttendanceDate.DayOfWeek != DayOfWeek.Saturday &&
                r.AttendanceDate.DayOfWeek != DayOfWeek.Sunday)
            .ToList();

        _attendanceReport.Clear();

        foreach (var employee in _employees)
        {
            var employeeRecords = periodRecords
                .Where(r => r.EmployeeId == employee.Id)
                .ToList();

            // Count each weekday only once per employee.
            int presentDays = employeeRecords
                .Select(r => r.AttendanceDate.Date)
                .Distinct()
                .Count();

            // Protect against inconsistent or duplicate data.
            presentDays = Math.Min(presentDays, workingDays);

            // Keep IP addresses available for search without adding
            // another visible table column.
            string ipAddresses = string.Join(" ",
                employeeRecords
                    .Select(r => r.IPAddress)
                    .Where(ip => !string.IsNullOrWhiteSpace(ip))
                    .Distinct());

            _attendanceReport.Add(new AttendanceSummaryRowViewModel
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.UserName,
                    PresentDays = presentDays,
                    AbsentDays = workingDays - presentDays,
                    WorkingDays = workingDays,
                    IPAddress = ipAddresses
                });
        }

        _attendanceReportView.Refresh();
    }

    // Counts the weekdays between the start and end dates, excluding weekends.
    private static int CountWeekdays(DateTime startDate,DateTime endDate)
    {
        int count = 0;

        for (DateTime date = startDate.Date;
             date <= endDate.Date;
             date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday &&
                date.DayOfWeek != DayOfWeek.Sunday)
            {
                count++;
            }
        }

        return count;
    }

    #endregion
}

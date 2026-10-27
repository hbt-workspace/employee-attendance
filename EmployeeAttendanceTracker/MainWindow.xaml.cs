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
        else if (_currentPage == "Attendance")
        {
            _historyView?.Refresh();
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

        PageTitle.Text = page switch
        {
            "Dashboard" => "Dashboard",
            "Employees" => "Employees",
            _ => "Attendance History"
        };

        PageSubtitle.Text = page switch
        {
            "Dashboard" => "Your team's attendance at a glance.",
            "Employees" => "View employee records and account status.",
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

    private static Color ColorFromHex(string hex)
    {
        return (Color)ColorConverter.ConvertFromString(hex);
    }

    #endregion
}

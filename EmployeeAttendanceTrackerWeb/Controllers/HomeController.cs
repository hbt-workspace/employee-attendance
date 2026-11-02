using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeAttendanceTrackerWeb.Models;
using EmployeeAttendanceTrackerWeb.Services;
using EmployeeAttendanceTrackerWeb.Models.ViewModels;

namespace EmployeeAttendanceTrackerWeb.Controllers;

public class HomeController : Controller
{
    #region Fields

    private readonly ILogger<HomeController> _logger;
    private readonly DashboardService _dashboardService;

    #endregion

    #region Constructor

    public HomeController(ILogger<HomeController> logger,DashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    #endregion

    #region Dashboard

    public async Task<IActionResult> Index()
    {
        DashboardViewModel dashboardData =
            await _dashboardService.GetDashboardDataAsync();

        return View(dashboardData);
    }

    #endregion

    #region Other Actions

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #endregion
}

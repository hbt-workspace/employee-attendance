using EmployeeAttendanceTrackerWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAttendanceTrackerWeb.Controllers;

// AttendanceController handles requests related to employee attendance.
public class AttendanceController : Controller
{
    #region Fields

    private readonly ILogger<AttendanceController> _logger;
    private readonly AttendanceService _attendanceService;

    #endregion

    #region Constructor

    public AttendanceController(ILogger<AttendanceController> logger, AttendanceService attendanceService)
    {
        _logger = logger;
        _attendanceService = attendanceService;
    }

    #endregion

    #region Attendance Actions

    public async Task<IActionResult> Index()
    {
        var attendanceData = await _attendanceService.GetTodayAttendanceAsync();
        
        return View(attendanceData);
    }
    
    #endregion
} 

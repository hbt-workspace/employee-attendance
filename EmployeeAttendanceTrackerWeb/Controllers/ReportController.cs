using EmployeeAttendanceTrackerWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAttendanceTrackerWeb.Controllers;

// ReportController handles requests related to employee attendance reports.
public class ReportController : Controller
{
    #region Fields

    private readonly ILogger<ReportController> _logger;
    private readonly AttendanceService _attendanceService;

    #endregion

    #region Constructor

    public ReportController(ILogger<ReportController> logger, AttendanceService attendanceService)
    {
        _logger = logger;
        _attendanceService = attendanceService;
    }

    #endregion

    #region Attendance Report

    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
    {
        // If only one date is entered, show validation error.
        if (fromDate.HasValue != toDate.HasValue)
        {
            ModelState.AddModelError(
                string.Empty,
                "Please select both From Date and To Date.");
        }

        // Validate date range.
        if (fromDate.HasValue && toDate.HasValue)
        {
            DateTime minimumDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime maximumDate = new DateTime(DateTime.Now.Year, 12, 31);

            if (fromDate.Value.Date > toDate.Value.Date)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "From Date cannot be greater than To Date.");
            }
            else if (fromDate.Value.Date < minimumDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"From Date cannot be less than 01 Jan {minimumDate.Year}.");
            }
            else if (toDate.Value.Date > maximumDate)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"To Date cannot be greater than 31 Dec {maximumDate.Year}.");
            }
        }

        // If validation fails, display the report with an error message.
        if (!ModelState.IsValid)
        {
            var report = await _attendanceService.GetAttendanceReportAsync();
            return View(report);
        }

        // Load filtered or complete attendance report.
        var attendanceReport = await _attendanceService.GetAttendanceReportAsync(fromDate,toDate);

        return View(attendanceReport);
    }

    #endregion
}

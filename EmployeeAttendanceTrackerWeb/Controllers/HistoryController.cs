using EmployeeAttendanceTrackerWeb.Models.ViewModels;
using EmployeeAttendanceTrackerWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAttendanceTrackerWeb.Controllers;

// Controller for managing attendance history and related views.
public class HistoryController : Controller
{
    #region Fields and Constructor

    private readonly AttendanceService _attendanceService;

    public HistoryController(AttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    #endregion

    #region Attendance History 

    // Displays attendance history with default or selected filters.
    public async Task<IActionResult> Index(int? employeeId, DateTime? fromDate, DateTime? toDate)
    {
        // Validate only when both dates are selected.
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            ModelState.AddModelError(string.Empty, "From Date cannot be later than To Date.");
        }

        var filter = new AttendanceHistoryFilterViewModel
        {
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var model = await _attendanceService.GetAttendanceHistoryAsync(filter);

        return View(model);
    }

    #endregion
}

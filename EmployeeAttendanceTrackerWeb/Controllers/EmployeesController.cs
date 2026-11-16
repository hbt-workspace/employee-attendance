using EmployeeAttendanceTrackerWeb.Data;
using EmployeeAttendanceTrackerWeb.Models.ViewModels;
using EmployeeAttendanceTrackerWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceTrackerWeb.Controllers;

// Controller for managing employee-related actions
public class EmployeesController : Controller
{
    #region Fields

    private readonly ILogger<EmployeesController> _logger;
    private readonly AppDbContext _context;

    #endregion

    #region Constructor

    public EmployeesController(ILogger<EmployeesController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    #endregion

    #region Employee Actions

    // Displays the list of employees.
    public async Task<IActionResult> Index()
    {
        var employees = await _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.UserName)
            .Select(u => new EmployeeViewModel
            {
                Id = u.Id,
                EmployeeName = u.UserName,
                IsActive = u.IsActive
            })
            .ToListAsync();

        return View(employees);
    }

    #endregion
}
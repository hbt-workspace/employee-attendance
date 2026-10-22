using EmployeeAttendanceService.Services;

#region Application Entry Point

// Initialize the attendance processing service.
var attendanceService = new AttendanceProcessingService();

// Execute the attendance workflow.
await attendanceService.RunAsync();

#endregion
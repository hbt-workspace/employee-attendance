namespace EmployeeAttendanceTracker.Models;

// Base Model for all models in the application.
public class BaseModel
{
    #region Common Properties

    // Common properties for all models
    public bool IsDeleted { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }

    #endregion
}
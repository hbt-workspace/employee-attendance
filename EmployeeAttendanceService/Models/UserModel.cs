namespace EmployeeAttendanceService.Models;

// User Model for the application.
public class UserModel : BaseModel
{
    #region Data-Model Properties

    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    #endregion
}
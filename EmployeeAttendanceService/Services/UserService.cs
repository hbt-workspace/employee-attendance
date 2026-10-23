using EmployeeAttendanceService.Data;
using EmployeeAttendanceService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAttendanceService.Services;

// User Service for the application.
public class UserService
{
    #region UserService Methods

    // Get or create user method for the application.
    public async Task<UserModel> GetOrCreateUserAsync()
    {
        // Get the current Windows username and create the database context.
        string windowsUserName = Environment.UserName;
        using var db = RuntimeDbContextFactory.CreateDbContext();

        // Find the existing user.
        var user = await db.Users.FirstOrDefaultAsync(u =>u.UserName == windowsUserName &&!u.IsDeleted);

        // Return the existing user if found.
        if (user != null)
        {
            return user;
        }

        // Create a new user if not found.
        user = new UserModel
        {
            UserName = windowsUserName,
            IsActive = true,
            IsDeleted = false,
            CreatedBy = 1,
            CreatedOn = DateTime.Now,
        };

        // Save the new user.
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    #endregion
}
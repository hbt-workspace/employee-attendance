using EmployeeAttendanceTrackerWeb.Data;
using EmployeeAttendanceTrackerWeb.Helpers;
using EmployeeAttendanceTrackerWeb.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<WorkingDayHelper>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<AttendanceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Https redirection & Routing
app.UseHttpsRedirection();
app.UseRouting();

// Use authorization
app.UseAuthorization();

// Use static files
app.MapStaticAssets();

// Default route for the admin panel
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Start the application
app.Run();

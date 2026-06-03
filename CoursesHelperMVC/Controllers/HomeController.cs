using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoursesHelperMVC.Models;
using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        return View(await BuildHomeModelAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<HomeViewModel> BuildHomeModelAsync()
    {
        if (User?.Identity?.IsAuthenticated != true)
        {
            return new HomeViewModel
            {
                DisplayName = "there",
                IntroText = "Sign in with your Coordinator, Instructor, or Trainee account to access your workspace.",
                Options =
                [
                    new HomeOptionViewModel
                    {
                        Category = "Access",
                        Title = "Login",
                        Description = "Open your role-based workspace.",
                        Controller = "Account",
                        Action = "Login"
                    },
                    new HomeOptionViewModel
                    {
                        Category = "Public",
                        Title = "Public Lookup",
                        Description = "Check trainee certification progress using the public lookup.",
                        Controller = "PublicLookup",
                        Action = "Index"
                    }
                ]
            };
        }

        var displayName = User.Identity?.Name ?? "there";
        var userId = _userManager.GetUserId(User);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var userInfo = await _context.UserInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(info => info.UserId == userId);

            if (userInfo is not null)
            {
                displayName = $"{userInfo.FirstName} {userInfo.LastName}";
            }
        }

        if (User.IsInRole("Coordinator"))
        {
            return new HomeViewModel
            {
                DisplayName = displayName,
                RoleName = "Coordinator",
                IntroText = "Manage the training catalogue, schedules, instructors, enrollments, and learner updates.",
                Options =
                [
                    new HomeOptionViewModel { Category = "Catalogue", Title = "Courses", Description = "Manage courses, prices, prerequisites, and course details.", Controller = "Courses", Action = "Index" },
                    new HomeOptionViewModel { Category = "Catalogue", Title = "Subjects", Description = "Organise courses by subject area.", Controller = "Subjects", Action = "Index" },
                    new HomeOptionViewModel { Category = "Delivery", Title = "Sessions", Description = "Schedule course sessions, rooms, instructors, and seats.", Controller = "CourseSessions", Action = "Index" },
                    new HomeOptionViewModel { Category = "Teaching", Title = "Instructors", Description = "Manage instructor expertise, availability, and assigned sessions.", Controller = "Instructors", Action = "Index" },
                    new HomeOptionViewModel { Category = "Lifecycle", Title = "Enrollments", Description = "Update statuses, payments, and trainee registration records.", Controller = "Enrollments", Action = "Index" },
                    new HomeOptionViewModel { Category = "Payments", Title = "Payments", Description = "Track due dates, balances, and overdue course payments.", Controller = "Payments", Action = "Index" },
                    new HomeOptionViewModel { Category = "Updates", Title = "Notifications", Description = "Review system notifications and coordinator updates.", Controller = "Notifications", Action = "Index" },
                    new HomeOptionViewModel { Category = "Public", Title = "Public Lookup", Description = "Open the public certification progress lookup.", Controller = "PublicLookup", Action = "Index" }
                ]
            };
        }

        if (User.IsInRole("Instructor"))
        {
            return new HomeViewModel
            {
                DisplayName = displayName,
                RoleName = "Instructor",
                IntroText = "Review assigned sessions, see enrolled trainees, and record pass/fail results.",
                Options =
                [
                    new HomeOptionViewModel { Category = "Overview", Title = "Dashboard", Description = "See your upcoming teaching schedule at a glance.", Controller = "Instructor", Action = "Dashboard" },
                    new HomeOptionViewModel { Category = "Teaching", Title = "My Sessions", Description = "Open assigned course sessions and trainee lists.", Controller = "Instructor", Action = "MySessions" },
                    new HomeOptionViewModel { Category = "Updates", Title = "Notifications", Description = "Review updates about enrollments, sessions, and assessments.", Controller = "Notifications", Action = "Index" },
                    new HomeOptionViewModel { Category = "Public", Title = "Public Lookup", Description = "Use the public certification progress lookup.", Controller = "PublicLookup", Action = "Index" }
                ]
            };
        }

        if (User.IsInRole("Trainee"))
        {
            return new HomeViewModel
            {
                DisplayName = displayName,
                RoleName = "Trainee",
                IntroText = "Browse sessions, follow your enrollments, and track certification progress.",
                Options =
                [
                    new HomeOptionViewModel { Category = "Overview", Title = "Dashboard", Description = "View active enrollments, balances, and certification summary.", Controller = "Trainee", Action = "Dashboard" },
                    new HomeOptionViewModel { Category = "Courses", Title = "Available Sessions", Description = "Browse upcoming sessions and request enrollment.", Controller = "Trainee", Action = "AvailableSessions" },
                    new HomeOptionViewModel { Category = "Lifecycle", Title = "My Enrollments", Description = "Track enrollment status, payments, and assessment results.", Controller = "Trainee", Action = "MyEnrollments" },
                    new HomeOptionViewModel { Category = "Certification", Title = "Certification Progress", Description = "Review completed and pending certification requirements.", Controller = "Trainee", Action = "CertificationProgress" },
                    new HomeOptionViewModel { Category = "Updates", Title = "Notifications", Description = "Review updates about sessions, payments, and results.", Controller = "Notifications", Action = "Index" },
                    new HomeOptionViewModel { Category = "Public", Title = "Public Lookup", Description = "Open the public certification progress lookup.", Controller = "PublicLookup", Action = "Index" }
                ]
            };
        }

        return new HomeViewModel
        {
            DisplayName = displayName,
            IntroText = "Choose an option below to continue.",
            Options =
            [
                new HomeOptionViewModel { Category = "Public", Title = "Public Lookup", Description = "Open the public certification progress lookup.", Controller = "PublicLookup", Action = "Index" }
            ]
        };
    }
}

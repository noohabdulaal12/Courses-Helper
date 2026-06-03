using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.ViewComponents;

public class NotificationsMenuViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public NotificationsMenuViewComponent(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = _userManager.GetUserId(HttpContext.User);
        if (userId is null)
        {
            return Content(string.Empty);
        }

        var unreadCount = await _context.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && n.ReadAtUtc == null);

        return View(unreadCount);
    }
}

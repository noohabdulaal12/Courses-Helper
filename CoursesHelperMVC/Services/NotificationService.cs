using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Services;

public interface INotificationService
{
    Task NotifyUserAsync(string userId, string title, string message, string? link = null);
    Task NotifyCoordinatorsAsync(string title, string message, string? link = null);
}

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task NotifyUserAsync(string userId, string title, string message, string? link = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        _context.Notifications.Add(CreateNotification(userId, title, message, link));
        await _context.SaveChangesAsync();
    }

    public async Task NotifyCoordinatorsAsync(string title, string message, string? link = null)
    {
        var coordinatorIds = await _context.Users
            .AsNoTracking()
            .Where(u => u.UserType == UserType.Coordinator)
            .Select(u => u.Id)
            .ToListAsync();

        foreach (var userId in coordinatorIds)
        {
            _context.Notifications.Add(CreateNotification(userId, title, message, link));
        }

        if (coordinatorIds.Count > 0)
        {
            await _context.SaveChangesAsync();
        }
    }

    private static Notification CreateNotification(string userId, string title, string message, string? link)
    {
        return new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Link = link,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}

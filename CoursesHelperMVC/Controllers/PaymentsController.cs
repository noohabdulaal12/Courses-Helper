using CoursesHelperMVC.Models;
using CoursesHelperWebAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

[Authorize(Roles = "Coordinator")]
public class PaymentsController : Controller
{
    private static readonly HashSet<string> _allowedFilters = ["All", "Overdue", "Unpaid", "Partial", "Paid"];
    private readonly AppDbContext _context;

    public PaymentsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string filter = "All")
    {
        if (!_allowedFilters.Contains(filter))
        {
            filter = "All";
        }

        var enrollments = await _context.TraineeSessions
            .AsNoTracking()
            .Include(e => e.Trainee)
                .ThenInclude(t => t.UserInfo)
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Course)
            .OrderBy(e => e.PaymentDueDate ?? DateOnly.MaxValue)
            .ThenBy(e => e.CourseSession.StartingDate)
            .ThenBy(e => e.Trainee.UserName)
            .ToListAsync();

        var payments = enrollments.Select(e => new PaymentSummaryViewModel
        {
            TraineeId = e.TraineeId,
            TraineeName = EnrollmentsController.FormatTrainee(e.Trainee),
            SessionId = e.SessionId,
            SessionDetails = EnrollmentsController.FormatSession(e.CourseSession),
            CoursePrice = e.CourseSession.Course.Price,
            AmountPaid = e.AmountPaid,
            PaymentDueDate = e.PaymentDueDate
        }).ToList();

        if (filter != "All")
        {
            payments = payments
                .Where(payment => PaymentStatusHelper.Format(payment.PaymentStatus) == filter)
                .ToList();
        }

        return View(new PaymentsIndexViewModel
        {
            Filter = filter,
            Payments = payments
        });
    }
}

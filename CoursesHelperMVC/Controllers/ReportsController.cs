using CoursesHelperMVC.Models;
using CoursesHelperWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Controllers;

public class ReportsController : Controller
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var enrollmentRevenueRows = await _context.TraineeSessions
            .AsNoTracking()
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Course)
            .Select(e => new
            {
                CourseName = e.CourseSession.Course.Name,
                CoursePrice = e.CourseSession.Course.Price,
                e.AmountPaid
            })
            .ToListAsync();

        var revenueByCourse = enrollmentRevenueRows
            .GroupBy(e => e.CourseName)
            .Select(g => new RevenueByCourseReportItem
            {
                CourseName = g.Key,
                EnrollmentCount = g.Count(),
                RevenueCollected = g.Sum(e => e.AmountPaid),
                OutstandingRevenue = g.Sum(e => Math.Max(0, e.CoursePrice - e.AmountPaid))
            })
            .OrderByDescending(r => r.RevenueCollected)
            .ThenBy(r => r.CourseName)
            .ToList();

        var model = new ReportsDashboardViewModel
        {
            TotalCourses = await _context.Courses.AsNoTracking().CountAsync(),
            TotalSubjects = await _context.Subjects.AsNoTracking().CountAsync(),
            TotalSessions = await _context.CourseSessions.AsNoTracking().CountAsync(),
            TotalEnrollments = await _context.TraineeSessions.AsNoTracking().CountAsync(),
            TotalRevenueCollected = enrollmentRevenueRows.Sum(e => e.AmountPaid),
            OutstandingRevenue = enrollmentRevenueRows.Sum(e => Math.Max(0, e.CoursePrice - e.AmountPaid)),
            EnrollmentsByCourse = await GetEnrollmentsByCourseAsync(),
            SessionsBySubject = await GetSessionsBySubjectAsync(),
            InstructorWorkload = await GetInstructorWorkloadAsync(),
            RevenueByCourse = revenueByCourse
        };

        return View(model);
    }

    private async Task<IReadOnlyList<EnrollmentsByCourseReportItem>> GetEnrollmentsByCourseAsync()
    {
        return await _context.TraineeSessions
            .AsNoTracking()
            .Include(e => e.CourseSession)
                .ThenInclude(s => s.Course)
            .GroupBy(e => e.CourseSession.Course.Name)
            .Select(g => new EnrollmentsByCourseReportItem
            {
                CourseName = g.Key,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .ThenBy(x => x.CourseName)
            .ToListAsync();
    }

    private async Task<IReadOnlyList<SessionsBySubjectReportItem>> GetSessionsBySubjectAsync()
    {
        return await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Course)
                .ThenInclude(c => c.Subject)
            .GroupBy(s => s.Course.Subject.Name)
            .Select(g => new SessionsBySubjectReportItem
            {
                SubjectName = g.Key,
                SessionCount = g.Count()
            })
            .OrderByDescending(x => x.SessionCount)
            .ThenBy(x => x.SubjectName)
            .ToListAsync();
    }

    private async Task<IReadOnlyList<InstructorWorkloadReportItem>> GetInstructorWorkloadAsync()
    {
        var rows = await _context.CourseSessions
            .AsNoTracking()
            .Include(s => s.Instructor)
                .ThenInclude(i => i.UserInfo)
            .Select(s => new
            {
                Instructor = s.Instructor,
                s.MaxSeats
            })
            .ToListAsync();

        return rows
            .GroupBy(r => ReportsDisplay.FormatInstructor(r.Instructor))
            .Select(g => new InstructorWorkloadReportItem
            {
                InstructorName = g.Key,
                SessionCount = g.Count(),
                TotalSeats = g.Sum(r => r.MaxSeats)
            })
            .OrderByDescending(x => x.SessionCount)
            .ThenBy(x => x.InstructorName)
            .ToList();
    }
}

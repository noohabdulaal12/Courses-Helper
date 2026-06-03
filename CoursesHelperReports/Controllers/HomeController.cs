using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursesHelperReports.Models;

namespace CoursesHelperReports.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ReportingApiClient _reportingApiClient;

    public HomeController(ILogger<HomeController> logger, ReportingApiClient reportingApiClient)
    {
        _logger = logger;
        _reportingApiClient = reportingApiClient;
    }

    [Authorize(Roles = "Coordinator")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var model = new ReportsDashboardViewModel
            {
                EnrollmentsByCourse = await _reportingApiClient.GetEnrollmentsByCourseAsync(),
                EnrollmentsBySubject = await _reportingApiClient.GetEnrollmentsBySubjectAsync(),
                InstructorWorkload = await _reportingApiClient.GetInstructorWorkloadAsync(),
                RevenueSummary = await _reportingApiClient.GetRevenueSummaryAsync(),
                CertificationCompletion = await _reportingApiClient.GetCertificationCompletionAsync()
            };

            return View(model);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Unable to load reports from the Web API.");
            ViewBag.ErrorMessage = "Reports could not be loaded from the Web API. Make sure the API project is running.";
            return View(new ReportsDashboardViewModel());
        }
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
}

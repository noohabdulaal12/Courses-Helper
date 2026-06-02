using System.Net;
using System.Text.Json;
using CoursesHelperMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursesHelperMVC.Controllers;

[AllowAnonymous]
public class PublicLookupController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PublicLookupController> _logger;

    public PublicLookupController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<PublicLookupController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new PublicLookupPageViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(PublicLookupPageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var baseUrl = _configuration["WebApi:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            model.ErrorMessage = "Web API base URL is not configured.";
            return View(model);
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);

        var traineeId = Uri.EscapeDataString(model.Request.TraineeId);
        var certificationId = model.Request.CertificationId;

        try
        {
            var response = await client.GetAsync($"/api/PublicLookup?traineeId={traineeId}&certificationId={certificationId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                model.ErrorMessage = "No matching certification record was found.";
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                model.ErrorMessage = "Please check the trainee ID and certification ID.";
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                model.ErrorMessage = "The lookup service is currently unavailable.";
                return View(model);
            }

            var stream = await response.Content.ReadAsStreamAsync();
            model.Result = await JsonSerializer.DeserializeAsync<PublicLookupResultViewModel>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (model.Result is null)
            {
                model.ErrorMessage = "The lookup service returned an empty response.";
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Public certification lookup API request failed.");
            model.ErrorMessage = "Could not connect to the Web API. Make sure the API project is running.";
        }

        return View(model);
    }
}

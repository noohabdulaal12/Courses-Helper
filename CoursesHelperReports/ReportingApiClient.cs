using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CoursesHelperReports.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class ReportingApiClient
{
    private const string TokenCacheKey = "reporting-api-jwt";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ReportingApiOptions _options;

    public ReportingApiClient(HttpClient httpClient, IMemoryCache cache, IOptions<ReportingApiOptions> options)
    {
        _httpClient = httpClient;
        _cache = cache;
        _options = options.Value;

        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }
    }

    public async Task<IReadOnlyList<EnrollmentByCourseReportItem>> GetEnrollmentsByCourseAsync()
    {
        return await GetReportAsync<EnrollmentByCourseReportItem>("/api/Reports/enrollments-by-course");
    }

    public async Task<IReadOnlyList<EnrollmentBySubjectReportItem>> GetEnrollmentsBySubjectAsync()
    {
        return await GetReportAsync<EnrollmentBySubjectReportItem>("/api/Reports/enrollments-by-subject");
    }

    public async Task<IReadOnlyList<InstructorWorkloadReportItem>> GetInstructorWorkloadAsync()
    {
        return await GetReportAsync<InstructorWorkloadReportItem>("/api/Reports/instructor-workload");
    }

    public async Task<IReadOnlyList<RevenueSummaryReportItem>> GetRevenueSummaryAsync()
    {
        return await GetReportAsync<RevenueSummaryReportItem>("/api/Reports/revenue-summary");
    }

    public async Task<IReadOnlyList<CertificationCompletionReportItem>> GetCertificationCompletionAsync()
    {
        return await GetReportAsync<CertificationCompletionReportItem>("/api/Reports/certification-completion");
    }

    private async Task<IReadOnlyList<T>> GetReportAsync<T>(string path)
    {
        var token = await GetTokenAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions);

        return result ?? [];
    }

    private async Task<string> GetTokenAsync()
    {
        if (_cache.TryGetValue(TokenCacheKey, out string? cachedToken) && !string.IsNullOrWhiteSpace(cachedToken))
        {
            return cachedToken;
        }

        var payload = JsonSerializer.Serialize(new
        {
            email = _options.Email,
            password = _options.Password
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(_options.LoginPath, content);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        var loginResponse = await JsonSerializer.DeserializeAsync<LoginResponse>(stream, JsonOptions)
            ?? throw new InvalidOperationException("API login did not return a valid response.");

        var cacheDuration = loginResponse.ExpiresAt > DateTime.UtcNow.AddMinutes(5)
            ? loginResponse.ExpiresAt - DateTime.UtcNow - TimeSpan.FromMinutes(2)
            : TimeSpan.FromMinutes(5);

        _cache.Set(TokenCacheKey, loginResponse.Token, cacheDuration);
        return loginResponse.Token;
    }

    private sealed class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}

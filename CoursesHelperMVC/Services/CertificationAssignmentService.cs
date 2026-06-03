using CoursesHelperWebAPI.Data;
using CoursesHelperWebAPI.Models.App;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperMVC.Services;

public interface ICertificationAssignmentService
{
    Task AutoAssignCompletedCertificationsAsync(string traineeId, int? newlyCompletedCourseId = null);
}

public class CertificationAssignmentService : ICertificationAssignmentService
{
    private readonly AppDbContext _context;

    public CertificationAssignmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AutoAssignCompletedCertificationsAsync(string traineeId, int? newlyCompletedCourseId = null)
    {
        var completedCourseIds = await _context.TraineeQualifications
            .Where(q => q.TraineeId == traineeId)
            .Select(q => q.CourseId)
            .ToListAsync();

        if (newlyCompletedCourseId.HasValue && !completedCourseIds.Contains(newlyCompletedCourseId.Value))
        {
            completedCourseIds.Add(newlyCompletedCourseId.Value);
        }

        if (completedCourseIds.Count == 0)
        {
            return;
        }

        var completedSet = completedCourseIds.ToHashSet();
        var certificationRequirements = await _context.CertificationCourses
            .GroupBy(cc => cc.CertificationId)
            .Select(group => new
            {
                CertificationId = group.Key,
                RequiredCourseIds = group.Select(cc => cc.CourseId).ToList()
            })
            .ToListAsync();

        foreach (var requirement in certificationRequirements)
        {
            if (requirement.RequiredCourseIds.Count == 0 || !requirement.RequiredCourseIds.All(completedSet.Contains))
            {
                continue;
            }

            var alreadyAssigned = await _context.TraineeCertifications.AnyAsync(tc =>
                tc.TraineeId == traineeId &&
                tc.CertificationId == requirement.CertificationId);

            if (!alreadyAssigned)
            {
                _context.TraineeCertifications.Add(new TraineeCertification
                {
                    TraineeId = traineeId,
                    CertificationId = requirement.CertificationId
                });
            }
        }
    }
}

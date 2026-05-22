using CoursesHelperWebAPI.Models.Identity;

namespace CoursesHelperMVC.Models;

public static class ReportsDisplay
{
    public static string FormatInstructor(User instructor)
    {
        if (instructor.UserInfo is not null)
        {
            return $"{instructor.UserInfo.FirstName} {instructor.UserInfo.LastName}";
        }

        return instructor.UserName ?? instructor.Email ?? instructor.Id;
    }
}

namespace CoursesHelperMVC.Models;

public class HomeViewModel
{
    public string DisplayName { get; set; } = "there";
    public string RoleName { get; set; } = string.Empty;
    public string IntroText { get; set; } = "Choose an option below to continue.";
    public IReadOnlyList<HomeOptionViewModel> Options { get; set; } = [];
}

public class HomeOptionViewModel
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = "Index";
}

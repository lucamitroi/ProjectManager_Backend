namespace ProjectManagerAPI.Models;

public partial class Project
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string ProjectTitle { get; set; }
    public string ProjectDescription { get; set; }
    public DateTime ProjectDate { get; set; }

    public Project()
    {
        ProjectTitle ??= "";
        ProjectDescription ??= "";
    }
}
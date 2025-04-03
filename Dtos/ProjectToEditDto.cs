namespace ProjectManagerAPI.Dtos;

public partial class ProjectToEditDto
{
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; }
    public string ProjectDescription { get; set; }
    public DateTime ProjectDate { get; set; }

    public ProjectToEditDto()
    {
        ProjectTitle ??= "";
        ProjectDescription ??= "";
    }
}
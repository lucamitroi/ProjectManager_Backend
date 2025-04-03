namespace ProjectManagerAPI.Dtos;

public partial class ProjectToAddDto
{
    public int UserId { get; set; }
    public string ProjectTitle { get; set; }
    public string ProjectDescription { get; set; }
    public DateTime ProjectDate { get; set; }

    public ProjectToAddDto()
    {
        ProjectTitle ??= "";
        ProjectDescription ??= "";
    }
}
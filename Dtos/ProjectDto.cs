using ProjectManagerAPI.Models;

namespace ProjectManagerAPI.Dtos;

public partial class ProjectDto
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public string ProjectTitle { get; set; }
    public string ProjectDescription { get; set; }
    public DateTime ProjectDate { get; set; }
    public IEnumerable<TaskModel> ListOfTasks { get; set; }

    public ProjectDto()
    {
        ProjectTitle ??= "";
        ProjectDescription ??= "";
        ListOfTasks = [];
    }
}
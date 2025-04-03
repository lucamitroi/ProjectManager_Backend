namespace ProjectManagerAPI.Dtos;

public partial class TaskToAddDto
{
    public int ProjectId { get; set; }
    public string TaskDescription { get; set; }

    public TaskToAddDto()
    {
        TaskDescription ??= "";
    }
}
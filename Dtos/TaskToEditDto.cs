namespace ProjectManagerAPI.Dtos;

public partial class TaskToEditDto
{
    public int TaskId { get; set; }
    public string TaskDescription { get; set; }

    public TaskToEditDto()
    {
        TaskDescription ??= "";
    }
}
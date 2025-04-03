namespace ProjectManagerAPI.Models;

public partial class TaskModel
{
    public int TaskId { get; set; }
    public int ProjectId { get; set; }
    public string TaskDescription { get; set; }

    public TaskModel()
    {
        TaskDescription ??= "";
    }
}
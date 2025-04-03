using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerAPI.Data;
using ProjectManagerAPI.Dtos;
using ProjectManagerAPI.Models;

namespace ProjectManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    public ProjectController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    // Endpoint used to get all the information related to a user Projects
    [HttpGet("UserProjects/{userId}")]
    public IEnumerable<ProjectDto> GetProjects(int userId)
    {
        string projectSql = @"
        SELECT [ProjectId], 
            [UserId],
            [ProjectTitle], 
            [ProjectDescription], 
            [ProjectDate] 
        FROM ProjectManagerSchema.Projects WHERE UserId = " + userId.ToString();
        IEnumerable<ProjectDto> projects = _dapper.LoadData<ProjectDto>(projectSql);

        foreach (ProjectDto project in projects)
        {
            string taskSql = @"
            SELECT [TaskId], 
                [ProjectId],
                [TaskDescription] 
            FROM ProjectManagerSchema.Tasks WHERE ProjectId = " + project.ProjectId.ToString();
            project.ListOfTasks = _dapper.LoadData<TaskModel>(taskSql);
        }
        return projects;
    }

    // Endpoint used to add a new Project
    [HttpPost("AddProject")]
    public IActionResult AddProject(ProjectToAddDto projectToAdd)
    {
        string findUserSql = "SELECT UserId FROM ProjectManagerSchema.Users WHERE UserId = " + projectToAdd.UserId.ToString();
        try
        {
            _dapper.LoadSingleData<int>(findUserSql);
        }
        catch (InvalidOperationException)
        {
            return StatusCode(404, "The user with the UserId = " + projectToAdd.UserId + " not found!");
        }

        DateTime formatedDate = DateTime.ParseExact(projectToAdd.ProjectDate.ToString(), "dd.MM.yyyy HH:mm:ss", null);

        string sql = @"
        INSERT INTO ProjectManagerSchema.Projects (
            [UserId],
            [ProjectTitle],
            [ProjectDescription],
            [ProjectDate]
        ) VALUES ("
        + projectToAdd.UserId +
        ", '" + projectToAdd.ProjectTitle +
        "', '" + projectToAdd.ProjectDescription +
        "', '" + formatedDate.ToString("MM-dd-yy") + "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to add the Project!");
    }

    // Endpoint used to edit a Project
    [HttpPut("EditProject")]
    public IActionResult EditProject(ProjectToEditDto projectToEdit)
    {
        string sql = @"
        UPDATE ProjectManagerSchema.Projects 
            SET ProjectDescription = '" + projectToEdit.ProjectDescription +
            "', ProjectTitle = '" + projectToEdit.ProjectTitle +
            "', ProjectDate = '" + projectToEdit.ProjectDate + "'" +
            @" WHERE ProjectId = " + projectToEdit.ProjectId.ToString() +
                    "AND UserId = " + this.User.FindFirst("userId")?.Value;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to edit the project!");
    }

    // Endpoint used to delete a project; it also delets all the related tasks
    [HttpDelete("DeleteProject/{projectId}")]
    public IActionResult DeleteProject(int projectId)
    {
        string sqlRemoveTasks = "DELETE FROM ProjectManagerSchema.Tasks WHERE ProjectId = " + projectId.ToString();
        string sqlRemoveProject = "DELETE FROM ProjectManagerSchema.Projects WHERE ProjectId = " + projectId.ToString();

        _dapper.ExecuteSql(sqlRemoveTasks);

        if (_dapper.ExecuteSql(sqlRemoveProject))
        {
            return Ok();
        }
        throw new Exception("Failed to Delete the Project");

    }

    // Endpoint used to get the tasks from a Project
    [HttpGet("ProjectTasks/{projectId}")]
    public IEnumerable<TaskModel> GetProjectTasks(int projectId)
    {
        string projectSql = @"
        SELECT [TaskId], 
            [ProjectId],
            [TaskDescription] 
        FROM ProjectManagerSchema.Tasks WHERE ProjectId = " + projectId.ToString();
        return _dapper.LoadData<TaskModel>(projectSql);
    }

    // Endpoint used to add a new task to a project
    [HttpPost("AddTask")]
    public IActionResult AddTask(TaskToAddDto taskToAdd)
    {
        string findProjectSql = "SELECT ProjectId FROM ProjectManagerSchema.Tasks WHERE ProjectId = " + taskToAdd.ProjectId.ToString();
        try
        {
            _dapper.LoadData<int>(findProjectSql);
        }
        catch (InvalidOperationException)
        {
            return StatusCode(404, "The project with the ProjectId = " + taskToAdd.ProjectId + " was not found!");
        }

        string sql = @"
            INSERT INTO ProjectManagerSchema.Tasks (
                [ProjectId],
                [TaskDescription]
            ) VALUES (" + taskToAdd.ProjectId + ", '" + taskToAdd.TaskDescription + "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to add the Task!");
    }

    // Endpoint used to edit a tasks data
    [HttpPut("EditTask")]
    public IActionResult EditTask(TaskToEditDto taskToEdit)
    {
        string sql = @"
        UPDATE ProjectManagerSchema.Tasks
            SET TaskDescription = '" + taskToEdit.TaskDescription + "'" +
            @" WHERE TaskId = " + taskToEdit.TaskId.ToString();

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to edit the project!");
    }

    // Endpoint used to delete a task based on the id
    [HttpDelete("DeleteTask/{taskId}")]
    public IActionResult DeleteTask(int taskId)
    {
        string sqlRemoveTasks = "DELETE FROM ProjectManagerSchema.Tasks WHERE TaskId = " + taskId.ToString();

        if (_dapper.ExecuteSql(sqlRemoveTasks))
        {
            return Ok();
        }
        throw new Exception("Failed to Delete the Task");

    }
}
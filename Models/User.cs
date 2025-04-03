namespace ProjectManagerAPI.Models;

public partial class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public User()
    {
        FirstName ??= "";
        LastName ??= "";
        Email ??= "";
    }
}
namespace ProjectManagerAPI.Dtos;

public partial class UserForRegistrationDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }

    public UserForRegistrationDto()
    {
        FirstName ??= "";
        LastName ??= "";
        Email ??= "";
        Password ??= "";
        PasswordConfirmation ??= "";
    }
}

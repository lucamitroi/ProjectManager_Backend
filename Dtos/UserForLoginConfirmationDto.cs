namespace ProjectManagerAPI.Dtos;

public partial class UserForLoginConfirmationDto
{
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set; }

    public UserForLoginConfirmationDto()
    {
        PasswordSalt ??= new byte[0];
        PasswordHash ??= new byte[0];
    }
}
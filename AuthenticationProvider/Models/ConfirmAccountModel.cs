namespace AuthenticationProvider.Models;

public class ConfirmAccountModel
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}



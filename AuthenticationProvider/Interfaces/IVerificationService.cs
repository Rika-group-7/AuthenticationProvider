namespace AuthenticationProvider.Interfaces
{
    public interface IVerificationService
    {
        Task SendVerificationRequest(string email, string token);
        Task<bool> ValidateVerificationCodeAsync(string email, string code);
    }
}
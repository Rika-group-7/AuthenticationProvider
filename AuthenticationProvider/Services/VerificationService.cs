using AuthenticationProvider.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;


namespace AuthenticationProvider.Services;
public class VerificationService(ServiceBusClient serviceBusClient) : IVerificationService
{
    private readonly ServiceBusClient _serviceBusClient = serviceBusClient;

    public async Task SendVerificationRequest(string email, string token)
    {
        try
        {
            await using var sender = _serviceBusClient.CreateSender("verification-queue");

            // Serialize the email and token
            var emailJson = JsonSerializer.Serialize(new { Email = email, Token = token });
            var message = new ServiceBusMessage(emailJson)
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(message);
            Console.WriteLine("Verification request sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending verification request: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> ValidateVerificationCodeAsync(string email, string code)
    {
        try
        {
            var client = new HttpClient();

            var validateRequest = new { Email = email, Code = code };

            var requestUri = $"ValidateAPI";

            var response = await client.PostAsJsonAsync(requestUri, validateRequest);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating verification code: {ex.Message}");
            return false;
        }
    }
}

using AuthenticationProvider.Interfaces;
using Azure.Messaging.ServiceBus;
using System.Text.Json;


namespace AuthenticationProvider.Services;
public class VerificationService : IVerificationService
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly string _queueName;

    public VerificationService(IConfiguration configuration)
    {
        var connectionString = configuration["ServiceBus:ConnectionString"];
        _queueName = configuration["ServiceBus:QueueName"]!;
        _serviceBusClient = new ServiceBusClient(connectionString);
    }

    public async Task SendVerificationRequest(string email, string token)
    {
        try
        {
            await using var sender = _serviceBusClient.CreateSender("verification-queue");

            var emailJson = JsonSerializer.Serialize(new { Email = email, Token = token });
            var message = new ServiceBusMessage(emailJson)
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(message);
            Console.WriteLine("Verification request sent successfully.");
        }
        catch (ServiceBusException sbEx)
        {
            Console.WriteLine($"Service Bus error: {sbEx.Message}");
            throw;
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
            var apiKey = Environment.GetEnvironmentVariable("validate-api-key");
            var requestUri = $"https://verificationprivider-rika.azurewebsites.net/api/verification?code={apiKey}";


            var response = await client.PostAsJsonAsync(requestUri, validateRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error validating verification code: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating verification code: {ex.Message}");
            return false;
        }
    }
}

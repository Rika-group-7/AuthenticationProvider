using AuthenticationProvider.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthenticationProvider.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly HttpClient _httpClient;
        private readonly string _verificationApiUrl;

        public VerificationService(ServiceBusClient serviceBusClient, HttpClient httpClient, IConfiguration configuration)
        {
            _serviceBusClient = serviceBusClient;
            _httpClient = httpClient;

            // Read verification API URL from configuration
            _verificationApiUrl = configuration["VerificationApiUrl"]!;
            if (string.IsNullOrEmpty(_verificationApiUrl))
            {
                throw new ArgumentNullException(nameof(_verificationApiUrl), "Verification API URL is not configured.");
            }
        }

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
                var validateRequest = new { Email = email, Code = code };

                var response = await _httpClient.PostAsJsonAsync(_verificationApiUrl, validateRequest);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating verification code: {ex.Message}");
                return false;
            }
        }
    }
}

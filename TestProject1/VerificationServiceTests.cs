namespace TestProject1;

using Moq;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xunit;
using AuthenticationProvider.Services;

public class VerificationServiceTests
{
    [Fact]
    public async Task SendVerificationRequest_ShouldSendServiceBusMessage()
    {
        // Arrange
        var mockServiceBusClient = new Mock<ServiceBusClient>();
        var mockSender = new Mock<ServiceBusSender>();

        mockServiceBusClient.Setup(client => client.CreateSender(It.IsAny<string>()))
                            .Returns(mockSender.Object);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { { "ServiceBus:QueueName", "verification-queue" } })
            .Build();

        var service = new VerificationService(mockServiceBusClient.Object);

        // Act
        await service.SendVerificationRequest("test@example.com", "testToken");

        // Assert
        mockSender.Verify(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), default), Times.Once);
    }
}

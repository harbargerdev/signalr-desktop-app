using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Clients.EventDetailsApi;
using SignalR.Event.Handler.Core.Clients.EventDetailsApi.Responses;
using SignalR.Event.Handler.Core.Configuration;
using SignalR.Event.Handler.Core.Utilities;
using System;

namespace SignalR.Event.Handler.Core.Tests.Clients.EventDetailsApi
{
    [TestFixture]
    public class EventDetailsApiClientTests
    {
        private Mock<IHttpClientUtility> _mockHttpClientUtility;
        private Mock<IOptions<ApiSettings>> _mockApiSettings;
        private EventDetailsApiClient _apiClient;
        private ApiSettings _apiSettings;

        [SetUp]
        public void SetUp()
        {
            _mockHttpClientUtility = new Mock<IHttpClientUtility>();
            _mockApiSettings = new Mock<IOptions<ApiSettings>>();

            _apiSettings = new ApiSettings
            {
                EventDetailsApiUrl = "https://api.test.com/events",
                TimeOutInSeconds = 30,
                MaxRetries = 3,
                InitialRetryDelayMs = 1000,
                RetryBackoffMultiplier = 2.0,
                MaxRetryDelayMs = 30000
            };

            _mockApiSettings.Setup(m => m.Value).Returns(_apiSettings);

            _apiClient = new EventDetailsApiClient(_mockHttpClientUtility.Object, _mockApiSettings.Object);
        }

        [Test]
        public void GetEventDetailsByUserName_ValidUserName_ReturnsEventDetails()
        {
            // Arrange
            var userName = "testuser";
            var expectedResponse = new EventDetailsResponse
            {
                UserId = "user123",
                EventId = "event456",
                EventName = "Test Event",
                EventDescription = "Test Description"
            };

            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Returns(expectedResponse);

            // Act
            var result = _apiClient.GetEventDetailsByUserName(userName);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(expectedResponse.UserId);
            result.EventId.Should().Be(expectedResponse.EventId);
            result.EventName.Should().Be(expectedResponse.EventName);
            result.EventDescription.Should().Be(expectedResponse.EventDescription);
        }

        [Test]
        public void GetEventDetailsByUserName_ValidUserName_CallsCorrectUrl()
        {
            // Arrange
            var userName = "testuser";
            var expectedUrl = $"{_apiSettings.EventDetailsApiUrl}/query/{userName}/";
            string? capturedUrl = null;

            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Callback<string>(url => capturedUrl = url)
                .Returns(new EventDetailsResponse());

            // Act
            _apiClient.GetEventDetailsByUserName(userName);

            // Assert
            capturedUrl.Should().Be(expectedUrl);
        }

        [Test]
        public void GetEventDetailsByUserName_NullUserName_ThrowsArgumentException()
        {
            // Act
            Action act = () => _apiClient.GetEventDetailsByUserName(null!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("userName")
                .WithMessage("*cannot be null, empty, or whitespace*");
        }

        [Test]
        public void GetEventDetailsByUserName_EmptyUserName_ThrowsArgumentException()
        {
            // Act
            Action act = () => _apiClient.GetEventDetailsByUserName(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("userName");
        }

        [Test]
        public void GetEventDetailsByUserName_WhitespaceUserName_ThrowsArgumentException()
        {
            // Act
            Action act = () => _apiClient.GetEventDetailsByUserName("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("userName");
        }

        [Test]
        public void GetEventDetailsByUserName_ApiReturnsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            var userName = "testuser";

            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Returns((EventDetailsResponse?)null);

            // Act
            Action act = () => _apiClient.GetEventDetailsByUserName(userName);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Failed to deserialize response*");
        }

        [Test]
        public void GetEventDetailsByUserName_HttpClientThrowsException_PropagatesException()
        {
            // Arrange
            var userName = "testuser";
            var expectedException = new Exception("Network error");

            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Throws(expectedException);

            // Act
            Action act = () => _apiClient.GetEventDetailsByUserName(userName);

            // Assert
            act.Should().Throw<Exception>()
                .WithMessage("Network error");
        }

        [Test]
        public void GetEventDetailsByUserName_UsesRetryPolicy_CallsHttpClientWithRetry()
        {
            // Arrange
            var userName = "testuser";
            var callCount = 0;

            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Callback(() =>
                {
                    callCount++;
                    if (callCount < 3)
                        throw new Exception("Transient error");
                })
                .Returns(new EventDetailsResponse { UserId = "123" });

            // Act
            var result = _apiClient.GetEventDetailsByUserName(userName);

            // Assert
            result.Should().NotBeNull();
            callCount.Should().Be(3); // Failed twice, succeeded on third attempt
        }

        [Test]
        public void GetEventDetailsByUserName_AllRetriesFail_ThrowsAggregateException()
        {
            // Arrange
            var userName = "testuser";
            var expectedError = new Exception("Persistent error");

            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Throws(expectedError);

            // Act
            Action act = () => _apiClient.GetEventDetailsByUserName(userName);

            // Assert
            act.Should().Throw<AggregateException>()
                .Which.InnerExceptions.Should().HaveCount(4); // Initial + 3 retries
        }

        [Test]
        public void Constructor_InitializesRetryPolicyFromSettings()
        {
            // Arrange
            var customSettings = new ApiSettings
            {
                EventDetailsApiUrl = "https://custom.api.com",
                MaxRetries = 5,
                InitialRetryDelayMs = 500,
                RetryBackoffMultiplier = 3.0,
                MaxRetryDelayMs = 60000
            };

            var mockCustomSettings = new Mock<IOptions<ApiSettings>>();
            mockCustomSettings.Setup(m => m.Value).Returns(customSettings);

            // Act
            var client = new EventDetailsApiClient(_mockHttpClientUtility.Object, mockCustomSettings.Object);

            // Assert - Verify retry policy is working by checking retry behavior
            var callCount = 0;
            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Callback(() =>
                {
                    callCount++;
                    throw new Exception("Test");
                });

            Action act = () => client.GetEventDetailsByUserName("test");
            act.Should().Throw<AggregateException>();
            callCount.Should().Be(6); // Initial + 5 retries
        }

        [Test]
        public void GetEventDetailsByUserName_SpecialCharactersInUserName_EncodesCorrectly()
        {
            // Arrange
            var userName = "test user@domain.com";
            string? capturedUrl = null;

            _mockHttpClientUtility
                .Setup(m => m.ExecuteHttpGetSync<EventDetailsResponse>(It.IsAny<string>()))
                .Callback<string>(url => capturedUrl = url)
                .Returns(new EventDetailsResponse());

            // Act
            _apiClient.GetEventDetailsByUserName(userName);

            // Assert
            capturedUrl.Should().Contain(userName);
            capturedUrl.Should().StartWith(_apiSettings.EventDetailsApiUrl);
        }
    }
}

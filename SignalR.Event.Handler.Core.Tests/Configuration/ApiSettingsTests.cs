using FluentAssertions;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Configuration;

namespace SignalR.Event.Handler.Core.Tests.Configuration
{
    [TestFixture]
    public class ApiSettingsTests
    {
        [Test]
        public void ApiSettings_DefaultConstructor_InitializesWithDefaults()
        {
            // Act
            var settings = new ApiSettings();

            // Assert
            settings.EventDetailsApiUrl.Should().BeEmpty();
            settings.TimeOutInSeconds.Should().Be(30);
            settings.MaxRetries.Should().Be(3);
            settings.InitialRetryDelayMs.Should().Be(1000);
            settings.RetryBackoffMultiplier.Should().Be(2.0);
            settings.MaxRetryDelayMs.Should().Be(30000);
        }

        [Test]
        public void ApiSettings_SetEventDetailsApiUrl_StoresValue()
        {
            // Arrange
            var settings = new ApiSettings();
            var url = "https://api.example.com/events";

            // Act
            settings.EventDetailsApiUrl = url;

            // Assert
            settings.EventDetailsApiUrl.Should().Be(url);
        }

        [Test]
        public void ApiSettings_SetTimeOutInSeconds_StoresValue()
        {
            // Arrange
            var settings = new ApiSettings();

            // Act
            settings.TimeOutInSeconds = 60;

            // Assert
            settings.TimeOutInSeconds.Should().Be(60);
        }

        [Test]
        public void ApiSettings_SetMaxRetries_StoresValue()
        {
            // Arrange
            var settings = new ApiSettings();

            // Act
            settings.MaxRetries = 5;

            // Assert
            settings.MaxRetries.Should().Be(5);
        }

        [Test]
        public void ApiSettings_SetInitialRetryDelayMs_StoresValue()
        {
            // Arrange
            var settings = new ApiSettings();

            // Act
            settings.InitialRetryDelayMs = 2000;

            // Assert
            settings.InitialRetryDelayMs.Should().Be(2000);
        }

        [Test]
        public void ApiSettings_SetRetryBackoffMultiplier_StoresValue()
        {
            // Arrange
            var settings = new ApiSettings();

            // Act
            settings.RetryBackoffMultiplier = 3.0;

            // Assert
            settings.RetryBackoffMultiplier.Should().Be(3.0);
        }

        [Test]
        public void ApiSettings_SetMaxRetryDelayMs_StoresValue()
        {
            // Arrange
            var settings = new ApiSettings();

            // Act
            settings.MaxRetryDelayMs = 60000;

            // Assert
            settings.MaxRetryDelayMs.Should().Be(60000);
        }

        [Test]
        public void ApiSettings_AllProperties_CanBeSet()
        {
            // Arrange & Act
            var settings = new ApiSettings
            {
                EventDetailsApiUrl = "https://api.example.com/events",
                TimeOutInSeconds = 45,
                MaxRetries = 4,
                InitialRetryDelayMs = 1500,
                RetryBackoffMultiplier = 2.5,
                MaxRetryDelayMs = 45000
            };

            // Assert
            settings.EventDetailsApiUrl.Should().Be("https://api.example.com/events");
            settings.TimeOutInSeconds.Should().Be(45);
            settings.MaxRetries.Should().Be(4);
            settings.InitialRetryDelayMs.Should().Be(1500);
            settings.RetryBackoffMultiplier.Should().Be(2.5);
            settings.MaxRetryDelayMs.Should().Be(45000);
        }

        [Test]
        public void ApiSettings_ZeroRetries_AllowsValue()
        {
            // Arrange
            var settings = new ApiSettings();

            // Act
            settings.MaxRetries = 0;

            // Assert
            settings.MaxRetries.Should().Be(0);
        }

        [Test]
        public void ApiSettings_NegativeRetries_AllowsValue()
        {
            // Arrange
            var settings = new ApiSettings();

            // Act
            settings.MaxRetries = -1;

            // Assert
            settings.MaxRetries.Should().Be(-1);
        }
    }
}

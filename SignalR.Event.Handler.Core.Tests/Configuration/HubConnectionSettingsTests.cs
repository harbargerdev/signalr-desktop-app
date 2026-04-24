using FluentAssertions;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Configuration;

namespace SignalR.Event.Handler.Core.Tests.Configuration
{
    [TestFixture]
    public class HubConnectionSettingsTests
    {
        [Test]
        public void HubConnectionSettings_DefaultConstructor_InitializesWithDefaults()
        {
            // Act
            var settings = new HubConnectionSettings();

            // Assert
            settings.ServerUrl.Should().BeEmpty();
        }

        [Test]
        public void HubConnectionSettings_SetServerUrl_StoresValue()
        {
            // Arrange
            var settings = new HubConnectionSettings();
            var url = "https://signalr.example.com/eventhub";

            // Act
            settings.ServerUrl = url;

            // Assert
            settings.ServerUrl.Should().Be(url);
        }

        [Test]
        public void HubConnectionSettings_SetServerUrlToEmpty_AllowsValue()
        {
            // Arrange
            var settings = new HubConnectionSettings
            {
                ServerUrl = "https://signalr.example.com/eventhub"
            };

            // Act
            settings.ServerUrl = string.Empty;

            // Assert
            settings.ServerUrl.Should().BeEmpty();
        }

        [Test]
        public void HubConnectionSettings_SetServerUrlToNull_AllowsValue()
        {
            // Arrange
            var settings = new HubConnectionSettings
            {
                ServerUrl = "https://signalr.example.com/eventhub"
            };

            // Act
            settings.ServerUrl = null!;

            // Assert
            settings.ServerUrl.Should().BeNull();
        }

        [Test]
        public void HubConnectionSettings_LocalhostUrl_AcceptsValue()
        {
            // Arrange
            var settings = new HubConnectionSettings();
            var url = "https://localhost:5001/eventhub";

            // Act
            settings.ServerUrl = url;

            // Assert
            settings.ServerUrl.Should().Be(url);
        }

        [Test]
        public void HubConnectionSettings_HttpUrl_AcceptsValue()
        {
            // Arrange
            var settings = new HubConnectionSettings();
            var url = "http://signalr.example.com/eventhub";

            // Act
            settings.ServerUrl = url;

            // Assert
            settings.ServerUrl.Should().Be(url);
        }

        [Test]
        public void HubConnectionSettings_UrlWithPort_AcceptsValue()
        {
            // Arrange
            var settings = new HubConnectionSettings();
            var url = "https://signalr.example.com:8443/eventhub";

            // Act
            settings.ServerUrl = url;

            // Assert
            settings.ServerUrl.Should().Be(url);
        }

        [Test]
        public void HubConnectionSettings_UrlWithPath_AcceptsValue()
        {
            // Arrange
            var settings = new HubConnectionSettings();
            var url = "https://signalr.example.com/api/v1/eventhub";

            // Act
            settings.ServerUrl = url;

            // Assert
            settings.ServerUrl.Should().Be(url);
        }
    }
}

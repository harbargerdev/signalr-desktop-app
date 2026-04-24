using FluentAssertions;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Utilities;
using SignalR.Event.Handler.Core.Utilities.Extensions;

namespace SignalR.Event.Handler.Core.Tests.Utilities
{
    [TestFixture]
    public class ConnectionStatusTests
    {
        [Test]
        public void ConnectionStatus_DefaultValue_IsDisconnected()
        {
            // Arrange & Act
            var status = default(ConnectionStatus);

            // Assert
            status.Should().Be(ConnectionStatus.Disconnected);
            ((int)status).Should().Be(0);
        }

        [Test]
        public void ConnectionStatus_AllValues_HaveCorrectOrdinal()
        {
            // Assert
            ((int)ConnectionStatus.Disconnected).Should().Be(0);
            ((int)ConnectionStatus.Connecting).Should().Be(1);
            ((int)ConnectionStatus.Connected).Should().Be(2);
        }

        [Test]
        public void ToDisplayString_Disconnected_ReturnsDisconnected()
        {
            // Arrange
            var status = ConnectionStatus.Disconnected;

            // Act
            var result = status.ToDisplayString();

            // Assert
            result.Should().Be("Disconnected");
        }

        [Test]
        public void ToDisplayString_Connecting_ReturnsConnecting()
        {
            // Arrange
            var status = ConnectionStatus.Connecting;

            // Act
            var result = status.ToDisplayString();

            // Assert
            result.Should().Be("Connecting");
        }

        [Test]
        public void ToDisplayString_Connected_ReturnsConnected()
        {
            // Arrange
            var status = ConnectionStatus.Connected;

            // Act
            var result = status.ToDisplayString();

            // Assert
            result.Should().Be("Connected");
        }

        [Test]
        public void ToDisplayString_InvalidValue_ReturnsUnknown()
        {
            // Arrange
            var status = (ConnectionStatus)99;

            // Act
            var result = status.ToDisplayString();

            // Assert
            result.Should().Be("Unknown");
        }

        [Test]
        public void ToDisplayString_AllValidValues_ReturnNonEmpty()
        {
            // Arrange
            var statuses = new[]
            {
                ConnectionStatus.Disconnected,
                ConnectionStatus.Connecting,
                ConnectionStatus.Connected
            };

            // Act & Assert
            foreach (var status in statuses)
            {
                status.ToDisplayString().Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}

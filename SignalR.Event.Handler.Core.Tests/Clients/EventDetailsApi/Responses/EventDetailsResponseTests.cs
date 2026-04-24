using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Clients.EventDetailsApi.Responses;
using System.Runtime.Serialization;

namespace SignalR.Event.Handler.Core.Tests.Clients.EventDetailsApi.Responses
{
    [TestFixture]
    public class EventDetailsResponseTests
    {
        [Test]
        public void EventDetailsResponse_DefaultConstructor_InitializesProperties()
        {
            // Act
            var response = new EventDetailsResponse();

            // Assert
            response.Should().NotBeNull();
            response.UserId.Should().BeNull();
            response.EventId.Should().BeNull();
            response.EventName.Should().BeNull();
            response.EventDescription.Should().BeNull();
        }

        [Test]
        public void EventDetailsResponse_SetProperties_StoresValues()
        {
            // Arrange
            var response = new EventDetailsResponse
            {
                UserId = "user123",
                EventId = "event456",
                EventName = "Test Event",
                EventDescription = "Test Description"
            };

            // Assert
            response.UserId.Should().Be("user123");
            response.EventId.Should().Be("event456");
            response.EventName.Should().Be("Test Event");
            response.EventDescription.Should().Be("Test Description");
        }

        [Test]
        public void EventDetailsResponse_JsonSerialization_RoundTrip()
        {
            // Arrange
            var original = new EventDetailsResponse
            {
                UserId = "user123",
                EventId = "event456",
                EventName = "Test Event",
                EventDescription = "Test Description"
            };

            // Act
            var json = JsonConvert.SerializeObject(original);
            var deserialized = JsonConvert.DeserializeObject<EventDetailsResponse>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.UserId.Should().Be(original.UserId);
            deserialized.EventId.Should().Be(original.EventId);
            deserialized.EventName.Should().Be(original.EventName);
            deserialized.EventDescription.Should().Be(original.EventDescription);
        }

        [Test]
        public void EventDetailsResponse_JsonDeserialization_UsesJsonPropertyNames()
        {
            // Arrange
            var json = @"{
                ""userId"": ""user123"",
                ""eventId"": ""event456"",
                ""eventName"": ""Test Event"",
                ""eventDescription"": ""Test Description""
            }";

            // Act
            var response = JsonConvert.DeserializeObject<EventDetailsResponse>(json);

            // Assert
            response.Should().NotBeNull();
            response!.UserId.Should().Be("user123");
            response.EventId.Should().Be("event456");
            response.EventName.Should().Be("Test Event");
            response.EventDescription.Should().Be("Test Description");
        }

        [Test]
        public void EventDetailsResponse_JsonSerialization_UsesJsonPropertyNames()
        {
            // Arrange
            var response = new EventDetailsResponse
            {
                UserId = "user123",
                EventId = "event456",
                EventName = "Test Event",
                EventDescription = "Test Description"
            };

            // Act
            var json = JsonConvert.SerializeObject(response);

            // Assert
            json.Should().Contain("\"userId\":");
            json.Should().Contain("\"eventId\":");
            json.Should().Contain("\"eventName\":");
            json.Should().Contain("\"eventDescription\":");
        }

        [Test]
        public void GetObjectData_PopulatesSerializationInfo()
        {
            // Arrange
            var response = new EventDetailsResponse
            {
                UserId = "user123",
                EventId = "event456",
                EventName = "Test Event",
                EventDescription = "Test Description"
            };

            var serializationInfo = new SerializationInfo(typeof(EventDetailsResponse), new FormatterConverter());
            var streamingContext = new StreamingContext();

            // Act
            response.GetObjectData(serializationInfo, streamingContext);

            // Assert
            serializationInfo.GetString("userId").Should().Be("user123");
            serializationInfo.GetString("eventId").Should().Be("event456");
            serializationInfo.GetString("eventName").Should().Be("Test Event");
            serializationInfo.GetString("eventDescription").Should().Be("Test Description");
        }

        [Test]
        public void GetObjectData_NullValues_AddsNullToSerializationInfo()
        {
            // Arrange
            var response = new EventDetailsResponse();
            var serializationInfo = new SerializationInfo(typeof(EventDetailsResponse), new FormatterConverter());
            var streamingContext = new StreamingContext();

            // Act
            response.GetObjectData(serializationInfo, streamingContext);

            // Assert
            serializationInfo.GetString("userId").Should().BeNull();
            serializationInfo.GetString("eventId").Should().BeNull();
            serializationInfo.GetString("eventName").Should().BeNull();
            serializationInfo.GetString("eventDescription").Should().BeNull();
        }

        [Test]
        public void EventDetailsResponse_EmptyStrings_Stored()
        {
            // Arrange
            var response = new EventDetailsResponse
            {
                UserId = string.Empty,
                EventId = string.Empty,
                EventName = string.Empty,
                EventDescription = string.Empty
            };

            // Assert
            response.UserId.Should().BeEmpty();
            response.EventId.Should().BeEmpty();
            response.EventName.Should().BeEmpty();
            response.EventDescription.Should().BeEmpty();
        }

        [Test]
        public void EventDetailsResponse_JsonDeserialization_MissingFields_SetsNull()
        {
            // Arrange
            var json = @"{ ""userId"": ""user123"" }";

            // Act
            var response = JsonConvert.DeserializeObject<EventDetailsResponse>(json);

            // Assert
            response.Should().NotBeNull();
            response!.UserId.Should().Be("user123");
            response.EventId.Should().BeNull();
            response.EventName.Should().BeNull();
            response.EventDescription.Should().BeNull();
        }

        [Test]
        public void EventDetailsResponse_JsonDeserialization_ExtraFields_Ignored()
        {
            // Arrange
            var json = @"{
                ""userId"": ""user123"",
                ""eventId"": ""event456"",
                ""eventName"": ""Test Event"",
                ""eventDescription"": ""Test Description"",
                ""extraField"": ""should be ignored""
            }";

            // Act
            var response = JsonConvert.DeserializeObject<EventDetailsResponse>(json);

            // Assert
            response.Should().NotBeNull();
            response!.UserId.Should().Be("user123");
            response.EventId.Should().Be("event456");
        }

        [Test]
        public void EventDetailsResponse_ImplementsISerializable()
        {
            // Arrange
            var response = new EventDetailsResponse();

            // Assert
            response.Should().BeAssignableTo<ISerializable>();
        }
    }
}

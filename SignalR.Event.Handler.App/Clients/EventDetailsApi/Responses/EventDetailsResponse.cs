using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace SignalR.Event.Handler.App.Clients.EventDetailsApi.Responses
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EventDetailsResponse : ISerializable
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("eventId")]
        public string EventId { get; set; }
        [JsonProperty("eventName")]
        public string EventName { get; set; }
        [JsonProperty("eventDescription")]
        public string EventDescription { get; set; }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the current object.
        /// </summary>
        /// <param name="info">The SerializationInfo to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("userId", UserId);
            info.AddValue("eventId", EventId);
            info.AddValue("eventName", EventName);
            info.AddValue("eventDescription", EventDescription);
        }
    }
}

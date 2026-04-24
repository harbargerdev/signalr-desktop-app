using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalR.Event.Handler.Core.Clients.EventDetailsApi.Responses
{
    public class EventSummary
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("eventType")]
        public string EventType { get; set; }
    }
}

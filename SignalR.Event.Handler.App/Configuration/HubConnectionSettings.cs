namespace SignalR.Event.Handler.App.Configuration
{
    /// <summary>
    /// Represents the configuration settings required to establish a connection to a SignalR hub.
    /// </summary>
    public class HubConnectionSettings
    {
        /// <summary>
        /// Gets or sets the server URL used to connect to the SignalR hub.
        /// </summary>
        public string ServerUrl { get; set; } = string.Empty;
    }
}

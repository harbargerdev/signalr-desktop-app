namespace SignalR.Event.Handler.App.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for ConnectionStatus enum.
    /// </summary>
    public static class ConnectionStatusExtensions
    {
        /// <summary>
        /// Gets the display string for the connection status.
        /// </summary>
        /// <param name="status">The connection status.</param>
        /// <returns>The friendly display string.</returns>
        public static string ToDisplayString(this ConnectionStatus status)
        {
            return status switch
            {
                ConnectionStatus.Disconnected => "Disconnected",
                ConnectionStatus.Connecting => "Connecting",
                ConnectionStatus.Connected => "Connected",
                _ => "Unknown"
            };
        }
    }
}

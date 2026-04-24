using System.ComponentModel;

namespace SignalR.Event.Handler.Core.Utilities
{
    /// <summary>
    /// Represents the connection status of the SignalR hub.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// The connection is disconnected.
        /// </summary>
        [Description("Disconnected")]
        Disconnected = 0,

        /// <summary>
        /// The connection is in the process of connecting.
        /// </summary>
        [Description("Connecting")]
        Connecting = 1,

        /// <summary>
        /// The connection is established and active.
        /// </summary>
        [Description("Connected")]
        Connected = 2
    }
}

namespace SignalR.Event.Handler.Core.Configuration
{
    /// <summary>
    /// Configuration settings for API clients.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Gets or sets the Event Details API base URL.
        /// </summary>
        public string EventDetailsApiUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the HTTP request timeout in seconds.
        /// </summary>
        public int TimeOutInSeconds { get; set; } = 30;

        /// <summary>
        /// Gets or sets the maximum number of retry attempts.
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the initial delay between retries in milliseconds.
        /// </summary>
        public int InitialRetryDelayMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the exponential backoff multiplier.
        /// </summary>
        public double RetryBackoffMultiplier { get; set; } = 2.0;

        /// <summary>
        /// Gets or sets the maximum delay between retries in milliseconds.
        /// </summary>
        public int MaxRetryDelayMs { get; set; } = 30000;
    }
}

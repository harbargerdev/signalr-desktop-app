using System;
using System.Threading.Tasks;

namespace SignalR.Event.Handler.App.Utilities
{
    /// <summary>
    /// Provides retry policy configuration for API operations.
    /// </summary>
    public class RetryPolicy
    {
        /// <summary>
        /// Gets or sets the maximum number of retry attempts.
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the initial delay between retries in milliseconds.
        /// </summary>
        public int InitialDelayMs { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the exponential backoff multiplier.
        /// </summary>
        public double BackoffMultiplier { get; set; } = 2.0;

        /// <summary>
        /// Gets or sets the maximum delay between retries in milliseconds.
        /// </summary>
        public int MaxDelayMs { get; set; } = 30000;

        /// <summary>
        /// Executes a synchronous action with retry logic and exponential backoff.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the action.</returns>
        /// <exception cref="AggregateException">Thrown when all retry attempts fail.</exception>
        public T Execute<T>(Func<T> action)
        {
            var exceptions = new System.Collections.Generic.List<Exception>();
            var currentDelay = InitialDelayMs;

            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);

                    if (attempt == MaxRetries)
                    {
                        throw new AggregateException($"Operation failed after {MaxRetries} retry attempts.", exceptions);
                    }

                    // Wait with exponential backoff
                    System.Threading.Thread.Sleep(currentDelay);

                    // Calculate next delay with exponential backoff
                    currentDelay = (int)Math.Min(currentDelay * BackoffMultiplier, MaxDelayMs);
                }
            }

            // This line should never be reached
            throw new AggregateException("Unexpected retry logic failure.", exceptions);
        }

        /// <summary>
        /// Executes an asynchronous action with retry logic and exponential backoff.
        /// </summary>
        /// <typeparam name="T">The return type of the action.</typeparam>
        /// <param name="action">The asynchronous action to execute.</param>
        /// <returns>A task representing the asynchronous operation with the result.</returns>
        /// <exception cref="AggregateException">Thrown when all retry attempts fail.</exception>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            var exceptions = new System.Collections.Generic.List<Exception>();
            var currentDelay = InitialDelayMs;

            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);

                    if (attempt == MaxRetries)
                    {
                        throw new AggregateException($"Operation failed after {MaxRetries} retry attempts.", exceptions);
                    }

                    // Wait with exponential backoff
                    await Task.Delay(currentDelay);

                    // Calculate next delay with exponential backoff
                    currentDelay = (int)Math.Min(currentDelay * BackoffMultiplier, MaxDelayMs);
                }
            }

            // This line should never be reached
            throw new AggregateException("Unexpected retry logic failure.", exceptions);
        }
    }
}

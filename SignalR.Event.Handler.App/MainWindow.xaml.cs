using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SignalR.Event.Handler.App.Clients.EventDetailsApi;
using SignalR.Event.Handler.App.Clients.EventDetailsApi.Responses;
using SignalR.Event.Handler.App.Configuration;
using SignalR.Event.Handler.App.Dialogs;
using SignalR.Event.Handler.App.Utilities;
using SignalR.Event.Handler.App.Utilities.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SignalR.Event.Handler.App
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ApiSettings _apiSettings;
        private readonly HubConnectionSettings _hubConnectionSettings;
        private readonly IEventDetailsApiClient _eventDetailsApiClient;

        private HubConnection? _hubConnection;
        private EventDetailsResponse? _latestEvent;
        private ConnectionStatus _currentConnectionStatus = Utilities.ConnectionStatus.Disconnected;
        private string _connectionStatusText = Utilities.ConnectionStatus.Disconnected.ToDisplayString();
        private SolidColorBrush _connectionStatusBrush;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the current user's username.
        /// </summary>
        public string UserName { get; } = Environment.UserName;

        /// <summary>
        /// Gets the latest event details received from SignalR.
        /// </summary>
        public EventDetailsResponse? LatestEvent
        {
            get => _latestEvent;
            private set
            {
                if (_latestEvent != value)
                {
                    _latestEvent = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the connection status text.
        /// </summary>
        public string ConnectionStatus
        {
            get => _connectionStatusText;
            private set
            {
                if (_connectionStatusText != value)
                {
                    _connectionStatusText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the connection status brush color.
        /// </summary>
        public SolidColorBrush ConnectionStatusBrush
        {
            get => _connectionStatusBrush;
            private set
            {
                if (_connectionStatusBrush != value)
                {
                    _connectionStatusBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainWindow class with injected dependencies.
        /// </summary>
        /// <param name="apiSettings">The API configuration settings.</param>
        /// <param name="hubConnectionSettings">The SignalR hub connection settings.</param>
        /// <param name="eventDetailsApiClient">The event details API client.</param>
        public MainWindow(
            IOptions<ApiSettings> apiSettings,
            IOptions<HubConnectionSettings> hubConnectionSettings,
            IEventDetailsApiClient eventDetailsApiClient)
        {
            InitializeComponent();

            _apiSettings = apiSettings.Value;
            _hubConnectionSettings = hubConnectionSettings.Value;
            _eventDetailsApiClient = eventDetailsApiClient;
            _connectionStatusBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);

            SetupHubConnection();
            _ = StartConnectionAsync();
        }

        private void SetupHubConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubConnectionSettings.ServerUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<EventDetailsResponse>("ReceiveEventDetails", (eventDetails) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    LatestEvent = eventDetails;
                });
            });

            _hubConnection.Closed += async (error) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateConnectionStatus(Utilities.ConnectionStatus.Disconnected);
                });
                await Task.Delay(5000);
                await StartConnectionAsync();
            };

            _hubConnection.Reconnecting += (error) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateConnectionStatus(Utilities.ConnectionStatus.Connecting);
                });
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateConnectionStatus(Utilities.ConnectionStatus.Connected);
                });
                return Task.CompletedTask;
            };
        }

        private async Task StartConnectionAsync()
        {
            if (_hubConnection == null)
                return;

            try
            {
                UpdateConnectionStatus(Utilities.ConnectionStatus.Connecting);
                await _hubConnection.StartAsync();
                UpdateConnectionStatus(Utilities.ConnectionStatus.Connected);
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus(Utilities.ConnectionStatus.Disconnected);
                Console.WriteLine("Error reconnecting to SignalR hub.");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
                await ShowErrorDialogAsync("An error occurred while trying to reconnect. Please try again.", ex);
            }
        }

        private void UpdateConnectionStatus(Utilities.ConnectionStatus status)
        {
            _currentConnectionStatus = status;

            ConnectionStatus = status.ToDisplayString();

            ConnectionStatusBrush = status switch
            {
                Utilities.ConnectionStatus.Connected => new SolidColorBrush(Microsoft.UI.Colors.Green),
                Utilities.ConnectionStatus.Connecting => new SolidColorBrush(Microsoft.UI.Colors.Orange),
                Utilities.ConnectionStatus.Disconnected => new SolidColorBrush(Microsoft.UI.Colors.Red),
                _ => new SolidColorBrush(Microsoft.UI.Colors.Gray)
            };
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void ForcePullButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable button during operation
            ForcePullButton.IsEnabled = false;

            try
            {
                var eventDetails = await Task.Run(() => _eventDetailsApiClient.GetEventDetailsByUserName(UserName));
                if (eventDetails != null)
                {
                    LatestEvent = eventDetails;
                }
                else
                {
                    await ShowErrorDialogAsync("The API returned no data. Please try again.", 
                        new InvalidOperationException("API call succeeded but returned null."));
                }
            }
            catch (AggregateException aggEx)
            {
                // Unwrap AggregateException to get the actual exception
                var innerException = aggEx.InnerException ?? aggEx;
                await ShowErrorDialogAsync("An error occurred calling the event service. Please try again.", innerException);
            }
            catch (Exception ex)
            {
                await ShowErrorDialogAsync("An error occurred calling the event service. Please try again.", ex);
            }
            finally
            {
                // Re-enable button after operation
                ForcePullButton.IsEnabled = true;
            }
        }

        private async void ReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_hubConnection != null)
            {
                try
                {
                    await _hubConnection.StopAsync();
                    SetupHubConnection();
                    await StartConnectionAsync();
                }
                catch (Exception)
                {
                    // Errors are handled in the StartConnectionAsync method, so we can ignore them here.
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Shows an error dialog with exception details.
        /// </summary>
        /// <param name="userMessage">The user-friendly message to display.</param>
        /// <param name="exception">The exception that occurred.</param>
        private async Task ShowErrorDialogAsync(string userMessage, Exception exception)
        {
            // Log to debug output for troubleshooting
            System.Diagnostics.Debug.WriteLine($"[ERROR] {userMessage}");
            System.Diagnostics.Debug.WriteLine($"[ERROR] Exception Type: {exception.GetType().FullName}");
            System.Diagnostics.Debug.WriteLine($"[ERROR] Message: {exception.Message}");
            System.Diagnostics.Debug.WriteLine($"[ERROR] Stack Trace: {exception.StackTrace}");

            var dialog = new ErrorDialog(userMessage, exception);
            await dialog.ShowAsync(Content.XamlRoot);
        }
    }
}

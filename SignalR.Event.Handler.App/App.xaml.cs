using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using SignalR.Event.Handler.Core.Clients.EventDetailsApi;
using SignalR.Event.Handler.Core.Configuration;
using SignalR.Event.Handler.Core.Utilities;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SignalR.Event.Handler.App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        private IHost? _host;

        /// <summary>
        /// Gets the service provider for dependency injection.
        /// </summary>
        public IServiceProvider Services => _host?.Services ?? throw new InvalidOperationException("Services not initialized");

        /// <summary>
        /// Gets the current application instance.
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            _host = ConfigureServices();
        }

        /// <summary>
        /// Configures services for dependency injection.
        /// </summary>
        private IHost ConfigureServices()
        {
            var builder = Host.CreateDefaultBuilder();

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);
            });

            builder.ConfigureServices((context, services) =>
            {
                // Register configuration sections (Singleton - configuration is immutable)
                services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));
                services.Configure<HubConnectionSettings>(context.Configuration.GetSection("HubConnection"));
                services.AddSingleton(context.Configuration);

                // Register utilities as transient to avoid state issues
                services.AddTransient<IHttpClientUtility, HttpClientUtility>();

                // Register API clients as transient to avoid state issues
                services.AddTransient<IEventDetailsApiClient, EventDetailsApiClient>();

                // Register MainWindow as transient so it can be resolved with dependencies
                services.AddTransient<MainWindow>();
            });

            return builder.Build();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Resolve MainWindow from DI container with all dependencies injected
            _window = Services.GetRequiredService<MainWindow>();
            _window.Activate();
        }
    }
}

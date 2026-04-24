# SignalR Desktop App

This is a prototype repository for a SignalR Desktop App. The application is designed to validate SignalR Hub and API connectivity for specific events and provide real-time updates to the user. It is an explorative project created for learning purposes only.

## Features

- Real-time communication using SignalR.
- Integration with APIs to fetch and display event details.
- Desktop application interface built with WPF.
- Error handling and retry policies for robust connectivity.
- Configuration management for API and Hub connections.

## Technologies Used

- **.NET 10.0**: Core framework for building the application.
- **SignalR**: For real-time communication.
- **WPF (Windows Presentation Foundation)**: For building the desktop application interface.
- **C#**: Programming language used for development.
- **XAML**: For designing the user interface.
- **Unit Testing**: Tests implemented using xUnit.

## Setup and Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/your-username/signalr-desktop-app.git
   ```

2. Navigate to the project directory:

   ```bash
   cd signalr-desktop-app
   ```

3. Open the solution file `SignalR.Event.Handler.slnx` in Visual Studio.

4. Restore NuGet packages:

   ```bash
   dotnet restore
   ```

5. Build the solution:

   ```bash
   dotnet build
   ```

6. Run the application:

   - Set `SignalR.Event.Handler.App` as the startup project.
   - Press `F5` to start debugging or `Ctrl+F5` to run without debugging.

## Usage

1. Configure the application settings in `AppSettings.json` located in the `SignalR.Event.Handler.App` folder.
2. Launch the application to connect to the SignalR Hub and APIs.
3. Monitor real-time updates and event details in the desktop interface.

## Folder Structure

```
SignalR.Event.Handler.App/
├── app.manifest
├── App.xaml
├── App.xaml.cs
├── AppSettings.json
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Package.appxmanifest
├── Assets/
├── Clients/
│   └── EventDetailsApi/
├── Configuration/
├── Dialogs/
│   └── ErrorDialog.xaml.cs
├── Properties/
│   └── launchSettings.json
├── Utilities/
│   └── Extensions/
```

## Contributing

This repository is for learning purposes only. Contributions are welcome but not expected. If you'd like to contribute, feel free to fork the repository, make your changes, and submit a pull request.

## License

This project is licensed under the terms of the [LICENSE](LICENSE) file.

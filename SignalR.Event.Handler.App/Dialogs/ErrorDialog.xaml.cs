using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Text;
using System;
using System.Linq;
using System.Text;

namespace SignalR.Event.Handler.App.Dialogs
{
    /// <summary>
    /// A dialog for displaying detailed error information.
    /// </summary>
    public sealed class ErrorDialog
    {
        private readonly string _userMessage;
        private readonly Exception _exception;

        /// <summary>
        /// Initializes a new instance of the ErrorDialog class.
        /// </summary>
        /// <param name="userMessage">The user-friendly error message.</param>
        /// <param name="exception">The exception that occurred.</param>
        public ErrorDialog(string userMessage, Exception exception)
        {
            _userMessage = userMessage;
            _exception = exception;
        }

        /// <summary>
        /// Gets the formatted error message, handling AggregateException specially.
        /// </summary>
        private string GetErrorMessage()
        {
            if (_exception is AggregateException aggEx && aggEx.InnerExceptions.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{aggEx.Message}");
                sb.AppendLine();
                sb.AppendLine("Inner Exceptions:");
                for (int i = 0; i < aggEx.InnerExceptions.Count; i++)
                {
                    sb.AppendLine($"  [{i + 1}] {aggEx.InnerExceptions[i].Message}");
                }
                return sb.ToString();
            }

            return _exception.Message ?? "No error message available.";
        }

        /// <summary>
        /// Gets the formatted stack trace, handling AggregateException specially.
        /// </summary>
        private string GetStackTrace()
        {
            if (_exception is AggregateException aggEx && aggEx.InnerExceptions.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== Aggregate Exception Stack Trace ===");
                sb.AppendLine(_exception.StackTrace ?? "No stack trace available.");
                sb.AppendLine();

                for (int i = 0; i < aggEx.InnerExceptions.Count; i++)
                {
                    sb.AppendLine($"=== Inner Exception [{i + 1}] Stack Trace ===");
                    sb.AppendLine(aggEx.InnerExceptions[i].StackTrace ?? "No stack trace available.");
                    if (i < aggEx.InnerExceptions.Count - 1)
                    {
                        sb.AppendLine();
                    }
                }
                return sb.ToString();
            }

            return _exception.StackTrace ?? "No stack trace available.";
        }

        /// <summary>
        /// Shows the error dialog.
        /// </summary>
        /// <param name="xamlRoot">The XamlRoot for the dialog.</param>
        public async System.Threading.Tasks.Task ShowAsync(XamlRoot xamlRoot)
        {
            var dialog = new ContentDialog
            {
                Title = "Error",
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var scrollViewer = new ScrollViewer
            {
                MaxHeight = 500,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var mainPanel = new StackPanel
            {
                Spacing = 15
            };

            // User Message
            var userMessageTextBlock = new TextBlock
            {
                Text = _userMessage,
                TextWrapping = TextWrapping.Wrap,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SystemErrorTextColor"]
            };
            mainPanel.Children.Add(userMessageTextBlock);

            // Error Message Section
            var errorMessagePanel = new StackPanel { Spacing = 5 };
            errorMessagePanel.Children.Add(new TextBlock
            {
                Text = "Error Message:",
                FontWeight = FontWeights.SemiBold,
                FontSize = 14
            });

            var errorMessageBorder = new Border
            {
                BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"]
            };

            errorMessageBorder.Child = new TextBlock
            {
                Text = GetErrorMessage(),
                TextWrapping = TextWrapping.Wrap,
                IsTextSelectionEnabled = true
            };
            errorMessagePanel.Children.Add(errorMessageBorder);
            mainPanel.Children.Add(errorMessagePanel);

            // Error Type Section
            var errorTypePanel = new StackPanel { Spacing = 5 };
            errorTypePanel.Children.Add(new TextBlock
            {
                Text = "Error Type:",
                FontWeight = FontWeights.SemiBold,
                FontSize = 14
            });

            var errorTypeBorder = new Border
            {
                BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"]
            };

            errorTypeBorder.Child = new TextBlock
            {
                Text = _exception.GetType().FullName ?? "Unknown",
                TextWrapping = TextWrapping.Wrap,
                IsTextSelectionEnabled = true
            };
            errorTypePanel.Children.Add(errorTypeBorder);
            mainPanel.Children.Add(errorTypePanel);

            // Stack Trace Section
            var stackTracePanel = new StackPanel { Spacing = 5 };
            stackTracePanel.Children.Add(new TextBlock
            {
                Text = "Stack Trace:",
                FontWeight = FontWeights.SemiBold,
                FontSize = 14
            });

            var stackTraceBorder = new Border
            {
                BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"]
            };

            var stackTraceScrollViewer = new ScrollViewer
            {
                MaxHeight = 200,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            stackTraceScrollViewer.Content = new TextBox
            {
                Text = GetStackTrace(),
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                BorderThickness = new Thickness(0),
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent),
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                FontSize = 12,
                Padding = new Thickness(10)
            };

            stackTraceBorder.Child = stackTraceScrollViewer;
            stackTracePanel.Children.Add(stackTraceBorder);
            mainPanel.Children.Add(stackTracePanel);

            scrollViewer.Content = mainPanel;
            dialog.Content = scrollViewer;

            await dialog.ShowAsync();
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Names.ViewModels;
using Names.Views;

namespace Names
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        private bool isRecordingTriggerKey = false;
        private ConsoleWindow _consoleWindow;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            _consoleWindow = new ConsoleWindow();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _consoleWindow.Close();
            this.Close();
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartRecordingCommand.Execute(null);

            // Update UI state
            recordButton.IsEnabled = false;
            stopButton.IsEnabled = true;

            // Attach keyboard event handler
            this.PreviewKeyDown += Window_PreviewKeyDown;

            // Attach mouse event handler
            //this.PreviewMouseDown += Window_PreviewMouseDown;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StopRecordingCommand.Execute(null);

            // Update UI state
            recordButton.IsEnabled = true;
            stopButton.IsEnabled = false;

            // Detach keyboard event handler
            this.PreviewKeyDown -= Window_PreviewKeyDown;

            // Detach mouse event handler
            //this.PreviewMouseDown -= Window_PreviewMouseDown;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveMacroCommand.Execute(null);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadMacroCommand.Execute(null);
            UpdateMacroUI();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearMacroCommand.Execute(null);
            UpdateMacroUI();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PlayMacroCommand.Execute(null);
        }

        private void StopMacro_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StopMacro();
        }

        private void RecordTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (!isRecordingTriggerKey)
            {
                isRecordingTriggerKey = true;
                recordTriggerButton.Content = "Press Key";
                ViewModel.WaitForTrigger = triggerKeyTextBox.Text != null;

                // Attach global key handler
                this.PreviewKeyDown += RecordTriggerKey_PreviewKeyDown;

                ViewModel.WriteToConsole("Press any key to set as trigger...");
            }
        }
        
        private void IncrementLoop_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoopCount++;
        }

        private void DecrementLoop_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.LoopCount > 1)
                ViewModel.LoopCount--;
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            // Hide the main content panel
            mainContent.Visibility = Visibility.Collapsed;

            // Show and navigate the content frame
            contentFrame.Visibility = Visibility.Visible;
            contentFrame.Navigate(new Views.SettingsPage());
        }

        public void ReturnToMain()
        {
            contentFrame.Content = null;
            contentFrame.Visibility = Visibility.Collapsed;
            mainContent.Visibility = Visibility.Visible;
        }

        private void OpenConsoleWindow_Click(object sender, RoutedEventArgs e)
        {
            // Show the window
            _consoleWindow.Show();
        }

        // Add this to enable dragging the windown
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.WriteToConsole($"Window size: {this.Width:0} x {this.Height:0}");
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.HandleKeyDown(e);

            // Since the ViewModel adds to its collection, we need to update our UI
            UpdateMacroUI();
        }        
        
        private void Window_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            ViewModel.HandleMouseDown(e);

            // Since the ViewModel adds to its collection, we need to update our UI
            UpdateMacroUI();
        }

        private void RecordTriggerKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (isRecordingTriggerKey)
            {
                // Capture the key
                ViewModel.TriggerKey = e.Key.ToString();

                // Stop recording
                isRecordingTriggerKey = false;
                recordTriggerButton.Content = "Set Key";

                // Remove handler
                this.PreviewKeyDown -= RecordTriggerKey_PreviewKeyDown;

                ViewModel.WriteToConsole($"Trigger key set to: {ViewModel.TriggerKey}");

                // Prevent the key from being processed further
                e.Handled = true;
            }
        }

        private void On_WindowInBackground(object sender, EventArgs e)
        {
            ViewModel.WriteToConsole($"Window in: Background");
        }

        private void On_WindowInForeground(object sender, EventArgs e)
        {
            ViewModel.WriteToConsole($"Window in: Foreground");
        }

        private void UpdateMacroUI()
        {
            // Clear existing UI
            EventsList.Children.Clear();

            // Create UI for each command in the ViewModel
            for (int i = 0; i < ViewModel.MacroCommands.Count; i++)
            {
                var command = ViewModel.MacroCommands[i];
                int index = i + 1;

                // Create the border container
                Border itemBorder = new Border
                {
                    Style = (Style)FindResource("EventItemStyle")
                };

                // Create the grid layout
                Grid itemGrid = new Grid();

                // Define columns
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Item number
                TextBlock numberBlock = new TextBlock
                {
                    Text = index.ToString("D3"),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA")),
                    FontFamily = new FontFamily("Consolas"),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(numberBlock, 0);

                // Command name/description
                TextBlock commandBlock = new TextBlock
                {
                    Text = FormatCommandText(command.KeyName),
                    Foreground = Brushes.White,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                Grid.SetColumn(commandBlock, 1);

                // Delay time (formatted as timestamp)
                TextBlock delayBlock = new TextBlock
                {
                    Text = FormatDelayAsTimestamp(command.DelayMs),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA")),
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0)
                };
                Grid.SetColumn(delayBlock, 2);

                // Delete button
                Button deleteButton = new Button
                {
                    Style = (Style)FindResource("ModernButton"),
                    Background = Brushes.Transparent,
                    Width = 28,
                    Height = 28,
                    Padding = new Thickness(6),
                    ToolTip = "Delete Event"
                };

                // Create the delete icon
                Path deletePath = new Path
                {
                    Data = (Geometry)FindResource("DeleteIcon"),
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5252")),
                    StrokeThickness = 1.5,
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round
                };
                deleteButton.Content = deletePath;

                // Store the index for use in the event handler
                int commandIndex = i;
                deleteButton.Click += (s, e) =>
                {
                    ViewModel.MacroCommands.RemoveAt(commandIndex);
                    UpdateMacroUI(); // Refresh the UI
                };

                Grid.SetColumn(deleteButton, 3);

                // Add all elements to the grid
                itemGrid.Children.Add(numberBlock);
                itemGrid.Children.Add(commandBlock);
                itemGrid.Children.Add(delayBlock);
                itemGrid.Children.Add(deleteButton);

                // Add the grid to the border
                itemBorder.Child = itemGrid;

                // Add the complete item to the stack panel
                EventsList.Children.Add(itemBorder);
            }

            // Update the status bar counts
            UpdateStatusBar(ViewModel.MacroCommands.Count);
        }

        // Helper method to format command text more descriptively
        private string FormatCommandText(string keyName)
        {
            if (keyName.StartsWith("Mouse"))
            {
                // For mouse commands, extract coordinates if present
                if (keyName.Contains("(") && keyName.Contains(")"))
                {
                    return keyName;
                }
                else
                {
                    return $"Mouse Click";
                }
            }
            else
            {
                // For keyboard commands
                return $"Key Press ({keyName})";
            }
        }

        // Helper method to format delay as a timestamp
        private string FormatDelayAsTimestamp(int delayMs)
        {
            TimeSpan time = TimeSpan.FromMilliseconds(delayMs);
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        }

        // Update the status bar with counts and timing information
        private void UpdateStatusBar(int commandCount)
        {
             eventsCountTextBlock.Text = $"Events: {commandCount}";

            // Calculate total duration
            int totalDuration = ViewModel.MacroCommands.Sum(cmd => cmd.DelayMs);
             durationTextBlock.Text = $"Duration: {totalDuration / 1000.0:F3}s";
        }
    }
}
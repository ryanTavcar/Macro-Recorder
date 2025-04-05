using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Names.ViewModels;

namespace Names
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
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
            if (!ViewModel.IsRecording)
            {
                ViewModel.SaveMacroCommand.Execute(null);
            }
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


        // Add this to enable dragging the window
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

        private void UpdateMacroUI()
        {
            // Clear existing UI
            textBoxContainer.Children.Clear();

            // Create UI for each command in the ViewModel
            foreach (var command in ViewModel.MacroCommands)
            {
                // Create the key text box
                TextBox keyTextBox = new TextBox
                {
                    Text = command.KeyName,
                    Width = 50,
                    Height = 30,
                    Margin = new Thickness(5),
                    IsReadOnly = true,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                // Create the delay text box with binding to the ViewModel
                TextBox delayTextBox = new TextBox
                {
                    Width = 50,
                    Height = 30,
                    Margin = new Thickness(5),
                    IsReadOnly = false,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                // Set the text manually - we should improve this with proper binding
                delayTextBox.Text = $"{command.DelayMs} ms";

                // Handle text changes to update the ViewModel
                delayTextBox.TextChanged += (s, e) => {
                    if (int.TryParse(delayTextBox.Text, out int delay))
                    {
                        command.DelayMs = delay;
                    }
         
                };

                // Add both text boxes to the container
                textBoxContainer.Children.Add(keyTextBox);
                textBoxContainer.Children.Add(delayTextBox);
            }
        }
    }
}
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

            // Set initial size label
            UpdateSizeLabel();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSizeLabel();
        }

        private void UpdateSizeLabel()
        {
            ViewModel.WriteToConsole($"Window size: {this.Width:0} x {this.Height:0}");
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartRecordingCommand.Execute(null);

            // Update UI state
            recordButton.IsEnabled = false;
            stopButton.IsEnabled = true;

            // Attach keyboard event handler
            this.PreviewKeyDown += Window_PreviewKeyDown;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StopRecordingCommand.Execute(null);

            // Update UI state
            recordButton.IsEnabled = true;
            stopButton.IsEnabled = false;

            // Detach keyboard event handler
            this.PreviewKeyDown -= Window_PreviewKeyDown;
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

        private void Clear_PreviewKeyDown(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearMacroCommand.Execute(null);
            textBoxContainer.Children.Clear();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.HandleKeyDown(e);

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
                delayTextBox.Text = command.DelayMs.ToString();

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
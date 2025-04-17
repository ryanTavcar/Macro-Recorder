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

            // Register for keyboard events for recording
            PreviewKeyDown += Window_PreviewKeyDown;

            // If using mouse events
            // PreviewMouseDown += Window_PreviewMouseDown;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Let the ViewModel handle all key press logic
            ViewModel.HandleKeyDown(e);
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Let the ViewModel handle mouse presses for recording
            ViewModel.HandleMouseDown(e);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Simple logging of window size changes
            ViewModel.WriteToConsole($"Window size: {this.Width:0} x {this.Height:0}");
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Enable window dragging
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void DisplayTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) // Check for double click
            {
                // Switch to edit mode
                ViewModel.IsEditingMacroName = true;

                // Focus the text box and select all text
                EditTextBox.Focus();
                EditTextBox.SelectAll();

                e.Handled = true;
            }
        }

        private void EditTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Exit edit mode when the textbox loses focus
            ViewModel.IsEditingMacroName = false;
        }

        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Save the changes and exit edit mode
                ViewModel.IsEditingMacroName = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                // Revert to the original value and exit edit mode
                EditTextBox.Text = ViewModel.CurrentMacroSequence.Name;
                ViewModel.IsEditingMacroName = false;
                e.Handled = true;
            }
        }

        private void DelayTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Check if playback is in progress
            if (ViewModel.IsPlaying)
            {
                // Don't allow editing during playback
                return;
            }

            if (e.ClickCount == 2) // Check for double click
            {
                // Get the MacroCommandViewModel from the Tag property
                var textBlock = (TextBlock)sender;
                var commandVM = (MacroCommandViewModel)textBlock.Tag;

                // Enter edit mode
                commandVM.IsEditingDelay = true;

                // Find the corresponding TextBox and focus it
                var parent = (Grid)textBlock.Parent;
                var textBox = parent.Children.OfType<TextBox>().FirstOrDefault();
                if (textBox != null)
                {
                    textBox.Focus();
                    textBox.SelectAll();
                }

                e.Handled = true;
            }
        }

        private void DelayTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;
            var commandVM = (MacroCommandViewModel)textBox.Tag;

            if (e.Key == Key.Enter)
            {
                // Try to parse the value
                if (int.TryParse(textBox.Text, out int newDelay))
                {
                    commandVM.DelayMs = newDelay;

                    // Update total duration in status bar
                    ViewModel.UpdateStatusInformation();
                }
                else
                {
                    // If parsing fails, revert to the original value
                    textBox.Text = commandVM.DelayMs.ToString();
                }

                // Exit edit mode
                commandVM.IsEditingDelay = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                // Revert to the original value
                textBox.Text = commandVM.DelayMs.ToString();

                // Exit edit mode
                commandVM.IsEditingDelay = false;
                e.Handled = true;
            }
        }

        private void DelayTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var commandVM = (MacroCommandViewModel)textBox.Tag;

            // Try to parse the value
            if (int.TryParse(textBox.Text, out int newDelay))
            {
                commandVM.DelayMs = newDelay;

                // Update total duration in status bar
                ViewModel.UpdateStatusInformation();
            }
            else
            {
                // If parsing fails, revert to the original value
                textBox.Text = commandVM.DelayMs.ToString();
            }

            // Exit edit mode
            commandVM.IsEditingDelay = false;
        }

        // Add this method to MainWindow.xaml.cs
        public void ReturnToMain()
        {

            // Clear the current page reference
            ViewModel.CurrentSettingsPage = null;

         // Update visibility
            ViewModel.IsSettingsPageVisible = false;

        }
    }
}
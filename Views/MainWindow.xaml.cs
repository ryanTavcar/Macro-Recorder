using System.Windows;
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
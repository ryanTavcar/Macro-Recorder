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
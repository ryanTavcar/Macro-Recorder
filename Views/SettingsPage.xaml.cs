using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Names.ViewModels;

namespace Names.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {

        // Provide a safer way to access the ViewModel
        private SettingsViewModel ViewModel
        {
            get
            {
                if (DataContext is SettingsViewModel vm)
                    return vm;

                // If DataContext is null or not a SettingsViewModel, create one
                var newViewModel = new SettingsViewModel();
                DataContext = newViewModel;
                return newViewModel;
            }
        }


        public SettingsPage()
        {
            InitializeComponent();
            // Only set the DataContext if it hasn't already been set from outside
            if (DataContext == null)
            {
                DataContext = new SettingsViewModel();
            }
        }

        /// <summary>
        /// Handle key press events for hotkey recording
        /// </summary>
        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Let the ViewModel handle key press logic
            ViewModel.HandleKeyDown(e);
        }

        // This method is needed to ensure compatibility with the original MainWindow implementation
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the main window and call the method to return to main view
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ReturnToMain();
            }
        }
    }
}
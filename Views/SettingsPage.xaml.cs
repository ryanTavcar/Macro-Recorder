using Names.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Names.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {

        private bool _showTooltips = true;
        private bool _isRecordingHotKey = false;
        private string _defaultRecordingHotKey = "LeftShift";
        private bool _randomizeTimingCheckBox = false;

        public SettingsPage()
        {
            InitializeComponent();

            // Altered default settings on initalization
            ShowTooltips.IsChecked = _showTooltips;
            RecordingHotkeyTextBox.Text = _defaultRecordingHotKey;
            RandomizeTimingCheckBox.IsChecked = _randomizeTimingCheckBox;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the main window and call the method to return to main view
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ReturnToMain();
            }
        }

        private void ShowTooltipsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Cast sender to CheckBox
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;

                // Log to the centralized service with the state
                LoggerService.Instance.Log($"Show tooltip {(isChecked ? "enabled" : "disabled")}");

                // You can also use the value to update settings or behavior
                 _showTooltips = isChecked;
            }
            e.Handled = true;
        }
        
        private void RecordHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecordingHotKey) return;
            _isRecordingHotKey = true;
            RecordingHotkeyTextBox.Text = "Press any key...";
            RecordingHotkeyTextBox.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            RecordHotkeyButton.Content = "Stop";

            // Register the key event handler
            this.PreviewKeyDown += StopRecordingHotKey;

            // Log to the centralized service with the state
            LoggerService.Instance.Log("Press any key to set as Hotkey...");
        }

        private void StopRecordingHotKey(object sender, KeyEventArgs e)
        {
            // Capture the key
            string hotkey = e.Key.ToString();
            _defaultRecordingHotKey = hotkey;
            RecordingHotkeyTextBox.Text = _defaultRecordingHotKey;

            // Stop recording
            _isRecordingHotKey = false;
            RecordingHotkeyTextBox.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            RecordHotkeyButton.Content = "Record";

            // Remove handler
            this.PreviewKeyDown -= StopRecordingHotKey;

            LoggerService.Instance.Log($"Trigger key set to: {hotkey}");

            // Prevent the key from being processed further
            e.Handled = true;
        }
    }
}

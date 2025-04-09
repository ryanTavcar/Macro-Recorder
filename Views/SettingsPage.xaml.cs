using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Names.Models;
using Names.Services;
using Names.ViewModels;

namespace Names.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        FileService _fileService;
        private bool _isRecordingHotKey = false;

        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
            _fileService = new FileService();
            SettingsManager.Instance.ForceReloadSettings();

            // Load default/saved settings on initalization
            RecordingHotkeyTextBox.Text = SettingsManager.Instance.Settings.RecordingHotkey;
            SelectComboBoxItem(MouseRecordingModeComboBox, SettingsManager.Instance.Settings.MouseRecordingMode);
            MousePrecisionSlider.Value = SettingsManager.Instance.Settings.MousePrecision;

            // For PlaybackSpeed
            double playbackSpeed = SettingsManager.Instance.Settings.PlaybackSpeed;
            foreach (ComboBoxItem item in PlaybackSpeedComboBox.Items)
            {
                if (Convert.ToDouble(item.Tag) == playbackSpeed)
                {
                    PlaybackSpeedComboBox.SelectedItem = item;
                    break;
                }
            }

            LoopDelayTextBox.Text = SettingsManager.Instance.Settings.LoopDelay.ToString();
            RandomizeTimingCheckBox.IsChecked = SettingsManager.Instance.Settings.RandomizeTiming;
            PlaybackHotkeyTextBox.Text = SettingsManager.Instance.Settings.PlaybackHotkey;
            SelectComboBoxItem(ThemeComboBox, SettingsManager.Instance.Settings.Theme);            
            ShowConsoleWindowCheckBox.IsChecked = SettingsManager.Instance.Settings.ShowConsoleWindow;
            StartWithWindowsCheckBox.IsChecked = SettingsManager.Instance.Settings.StartWithWindows;
            ShowTooltipsCheckBox.IsChecked = SettingsManager.Instance.Settings.ShowTooltips;
            DefaultSaveLocationTextBox.Text = SettingsManager.Instance.Settings.DefaultSaveLocation;
            SelectComboBoxItem(FileFormatComboBox, SettingsManager.Instance.Settings.FileFormat);
        }

        // helper method for matching combo box items to settings
        private void SelectComboBoxItem(ComboBox comboBox, string valueToFind)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == valueToFind)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        // back button button
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the main window and call the method to return to main view
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ReturnToMain();
            }
            e.Handled = true;
        }
        // record hotkey button
        private void RecordHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecordingHotKey) return;
            _isRecordingHotKey = true;
            RecordingHotkeyTextBox.Text = "Press any key...";
            RecordingHotkeyTextBox.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            RecordHotkeyButton.Content = "Stop";

            // Register the key event handler
            this.PreviewKeyDown += StopRecordHotkey_Click;
        }
        private void StopRecordHotkey_Click(object sender, KeyEventArgs e)
        {
            // Capture the key
            string hotkey = e.Key.ToString();

            // Stop recording
            _isRecordingHotKey = false;
            RecordingHotkeyTextBox.Text = hotkey;
            RecordingHotkeyTextBox.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            RecordHotkeyButton.Content = "Record";

            SettingsManager.Instance.Settings.RecordingHotkey = hotkey;
           
            // Remove handler
            this.PreviewKeyDown -= StopRecordHotkey_Click;

            // Prevent the key from being processed further
            e.Handled = true;
        }
        // mouse record mode selection
        private void MouseRecordMode_Select(object sender, RoutedEventArgs e) 
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content.ToString();
                SettingsManager.Instance.Settings.MouseRecordingMode = selectedOption;
            }
            e.Handled = true;
        }
        // mouse record precision slide
        private void MouseRecordPrecision_Slide(object sender, RoutedPropertyChangedEventArgs<double> e) 
        {
            double newValue = e.NewValue;
            SettingsManager.Instance.Settings.MousePrecision = newValue;

        }
        // playback speed selection
        private void PlaybackSpeed_select(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                double selectedOption = Convert.ToDouble(selectedItem.Tag);
                SettingsManager.Instance.Settings.PlaybackSpeed = selectedOption;
            }
            e.Handled = true;
        }
        // delay between commands textbox
        private void DelayBetweenCommands_TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            { 
                int delay = int.Parse(textBox.Text);
                SettingsManager.Instance.Settings.LoopDelay = delay;
            }
            e.Handled = true;
        }
        // randomize timing toggle
        private void RandomizeTimingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Cast sender to CheckBox
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;
                SettingsManager.Instance.Settings.RandomizeTiming = isChecked;
            }
            e.Handled = true;
        }
        // playback hotkey button
        private void RecordPlaybackHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecordingHotKey) return;
            _isRecordingHotKey = true;
            PlaybackHotkeyTextBox.Text = "Press any key...";
            PlaybackHotkeyTextBox.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            RecordPlaybackHotkeyButton.Content = "Stop";

            // Register the key event handler
            this.PreviewKeyDown += StopPlaybackHotkey_Click;
        }
        private void StopPlaybackHotkey_Click(object sender, KeyEventArgs e)
        {
            // Capture the key
            string hotkey = e.Key.ToString();

            // Stop recording
            _isRecordingHotKey = false;
            PlaybackHotkeyTextBox.Text = hotkey;
            PlaybackHotkeyTextBox.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            RecordPlaybackHotkeyButton.Content = "Record";

            SettingsManager.Instance.Settings.PlaybackHotkey = hotkey;
            
            // Remove handler
            this.PreviewKeyDown -= StopPlaybackHotkey_Click;

            // Prevent the key from being processed further
            e.Handled = true;
        }
        // change theme selection
        private void ChangeTheme_Select(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content.ToString();
                // Do something with the selected option
                SettingsManager.Instance.Settings.Theme = selectedOption;
            }
        }
        // show console window toggle
        private void ShowConsoleWindow_Click(object sender, RoutedEventArgs e)
        {
            // Cast sender to CheckBox
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;
                SettingsManager.Instance.Settings.ShowConsoleWindow = isChecked;
            }
            e.Handled = true;
        }
        // start with windows toggle
        private void StartWithWindows_Click(object sender, RoutedEventArgs e)
        {
            // Cast sender to CheckBox
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;
                SettingsManager.Instance.Settings.StartWithWindows = isChecked;
            }
            e.Handled = true;
        }
        // show tooltips toggle
        private void ShowTooltipsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Cast sender to CheckBox
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;
                SettingsManager.Instance.Settings.ShowTooltips = isChecked;
            }
            e.Handled = true;
        }
        // default save location button
        private void DefaultSaveLocation_Click(object sender, RoutedEventArgs e)
        { 
            string selectedPath = _fileService.BrowseSaveLocation();
            if (!string.IsNullOrEmpty(selectedPath))
            {
                SettingsManager.Instance.Settings.DefaultSaveLocation = selectedPath;
                DefaultSaveLocationTextBox.Text = selectedPath;

            }
        }
        // file format selection
        private void FileFormat_Select(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content.ToString();
                SettingsManager.Instance.Settings.FileFormat = selectedOption;
            }
            e.Handled = true;
        }
        // Update the SaveSettings_Click method to be asynchronous by adding the 'async' modifier
        private async void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Visual feedback - begin
            SaveSettingsButton.IsEnabled = false;
            SaveSettingsButton.Content = "Saving...";

            // Make SaveSettings return a Task
            await Task.Run(() => SettingsManager.Instance.SaveSettings());

            // Visual feedback - complete
            SaveConfirmationMessage.Visibility = Visibility.Visible;
            SaveSettingsButton.Content = "Saved!";

            // Give user time to see the confirmation (still needed)
            await Task.Delay(1000);

            // Reset
            SaveSettingsButton.IsEnabled = true;
            SaveSettingsButton.Content = "Save Settings";
            SaveConfirmationMessage.Visibility = Visibility.Collapsed;
        }
    }
}

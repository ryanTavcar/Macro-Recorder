using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Names.Models;
using Names.Services;

namespace Names.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly FileService _fileService;
        private bool _isRecordingHotKey = false;
        private string _recordingHotkey;
        private string _recordHotkeyButtonContent = "Record";
        private Brush _recordingHotkeyBackground = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        private string _mouseRecordingMode;
        private double _mousePrecision;
        private double _playbackSpeed;
        private int _loopDelay;
        private bool _randomizeTiming;
        private string _playbackHotkey;
        private string _recordPlaybackHotkeyButtonContent = "Record";
        private Brush _playbackHotkeyBackground = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        private string _theme;
        private bool _showConsoleWindow;
        private bool _startWithWindows;
        private bool _showTooltips;
        private string _defaultSaveLocation;
        private string _fileFormat;
        private bool _isButtonEnabled = true;
        private string _saveButtonContent = "Save Settings";
        private Visibility _saveConfirmationVisibility = Visibility.Collapsed;
        #endregion

        #region Public Properties
        public string RecordingHotkey
        {
            get => _recordingHotkey;
            set => SetProperty(ref _recordingHotkey, value);
        }

        public string RecordHotkeyButtonContent
        {
            get => _recordHotkeyButtonContent;
            set => SetProperty(ref _recordHotkeyButtonContent, value);
        }

        public Brush RecordingHotkeyBackground
        {
            get => _recordingHotkeyBackground;
            set => SetProperty(ref _recordingHotkeyBackground, value);
        }

        public string MouseRecordingMode
        {
            get => _mouseRecordingMode;
            set
            {
                if (SetProperty(ref _mouseRecordingMode, value))
                {
                    SettingsManager.Instance.Settings.MouseRecordingMode = value;
                }
            }
        }

        public double MousePrecision
        {
            get => _mousePrecision;
            set
            {
                if (SetProperty(ref _mousePrecision, value))
                {
                    SettingsManager.Instance.Settings.MousePrecision = value;
                }
            }
        }

        public double PlaybackSpeed
        {
            get => _playbackSpeed;
            set
            {
                if (SetProperty(ref _playbackSpeed, value))
                {
                    SettingsManager.Instance.Settings.PlaybackSpeed = value;
                }
            }
        }

        public int LoopDelay
        {
            get => _loopDelay;
            set
            {
                if (SetProperty(ref _loopDelay, value))
                {
                    SettingsManager.Instance.Settings.LoopDelay = value;
                }
            }
        }

        public bool RandomizeTiming
        {
            get => _randomizeTiming;
            set
            {
                if (SetProperty(ref _randomizeTiming, value))
                {
                    SettingsManager.Instance.Settings.RandomizeTiming = value;
                }
            }
        }

        public string PlaybackHotkey
        {
            get => _playbackHotkey;
            set => SetProperty(ref _playbackHotkey, value);
        }

        public string RecordPlaybackHotkeyButtonContent
        {
            get => _recordPlaybackHotkeyButtonContent;
            set => SetProperty(ref _recordPlaybackHotkeyButtonContent, value);
        }

        public Brush PlaybackHotkeyBackground
        {
            get => _playbackHotkeyBackground;
            set => SetProperty(ref _playbackHotkeyBackground, value);
        }

        public string Theme
        {
            get => _theme;
            set
            {
                if (SetProperty(ref _theme, value))
                {
                    SettingsManager.Instance.Settings.Theme = value;
                }
            }
        }

        public bool ShowConsoleWindow
        {
            get => _showConsoleWindow;
            set
            {
                if (SetProperty(ref _showConsoleWindow, value))
                {
                    SettingsManager.Instance.Settings.ShowConsoleWindow = value;
                }
            }
        }

        public bool StartWithWindows
        {
            get => _startWithWindows;
            set
            {
                if (SetProperty(ref _startWithWindows, value))
                {
                    SettingsManager.Instance.Settings.StartWithWindows = value;
                }
            }
        }

        public bool ShowTooltips
        {
            get => _showTooltips;
            set
            {
                if (SetProperty(ref _showTooltips, value))
                {
                    SettingsManager.Instance.Settings.ShowTooltips = value;
                }
            }
        }

        public string DefaultSaveLocation
        {
            get => _defaultSaveLocation;
            set => SetProperty(ref _defaultSaveLocation, value);
        }

        public string FileFormat
        {
            get => _fileFormat;
            set
            {
                if (SetProperty(ref _fileFormat, value))
                {
                    SettingsManager.Instance.Settings.FileFormat = value;
                }
            }
        }

        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set => SetProperty(ref _isButtonEnabled, value);
        }

        public string SaveButtonContent
        {
            get => _saveButtonContent;
            set => SetProperty(ref _saveButtonContent, value);
        }

        public Visibility SaveConfirmationVisibility
        {
            get => _saveConfirmationVisibility;
            set => SetProperty(ref _saveConfirmationVisibility, value);
        }

        public bool IsRecordingHotKey
        {
            get => _isRecordingHotKey;
            set => SetProperty(ref _isRecordingHotKey, value);
        }
        #endregion

        #region Commands
        public ICommand BackCommand { get; }
        public ICommand RecordHotkeyCommand { get; }
        public ICommand RecordPlaybackHotkeyCommand { get; }
        public ICommand BrowseSaveLocationCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        #endregion

        public SettingsViewModel()
        {
            _fileService = new FileService();
            SettingsManager.Instance.ForceReloadSettings();

            // Initialize commands
            BackCommand = new RelayCommand(_ => CloseSettings());
            RecordHotkeyCommand = new RelayCommand(_ => RecordHotkey());
            RecordPlaybackHotkeyCommand = new RelayCommand(_ => RecordPlaybackHotkey());
            BrowseSaveLocationCommand = new RelayCommand(_ => BrowseSaveLocation());
            SaveSettingsCommand = new RelayCommand(_ => SaveSettingsAsync());

            // Load settings values
            LoadSettings();
        }

        #region Public Methods
        public void HandleKeyDown(KeyEventArgs e)
        {
            if (IsRecordingHotKey)
            {
                // Capture the key
                string hotkey = e.Key.ToString();

                // Determine which hotkey is being recorded
                if (RecordHotkeyButtonContent == "Stop")
                {
                    // Recording the main recording hotkey
                    RecordingHotkey = hotkey;
                    RecordingHotkeyBackground = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                    RecordHotkeyButtonContent = "Record";
                    SettingsManager.Instance.Settings.RecordingHotkey = hotkey;
                }
                else if (RecordPlaybackHotkeyButtonContent == "Stop")
                {
                    // Recording the playback hotkey
                    PlaybackHotkey = hotkey;
                    PlaybackHotkeyBackground = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                    RecordPlaybackHotkeyButtonContent = "Record";
                    SettingsManager.Instance.Settings.PlaybackHotkey = hotkey;
                }

                IsRecordingHotKey = false;
                e.Handled = true;
            }
        }
        #endregion

        #region Private Methods
        private void LoadSettings()
        {
            RecordingHotkey = SettingsManager.Instance.Settings.RecordingHotkey;
            MouseRecordingMode = SettingsManager.Instance.Settings.MouseRecordingMode;
            MousePrecision = SettingsManager.Instance.Settings.MousePrecision;
            PlaybackSpeed = SettingsManager.Instance.Settings.PlaybackSpeed;
            LoopDelay = SettingsManager.Instance.Settings.LoopDelay;
            RandomizeTiming = SettingsManager.Instance.Settings.RandomizeTiming;
            PlaybackHotkey = SettingsManager.Instance.Settings.PlaybackHotkey;
            Theme = SettingsManager.Instance.Settings.Theme;
            ShowConsoleWindow = SettingsManager.Instance.Settings.ShowConsoleWindow;
            StartWithWindows = SettingsManager.Instance.Settings.StartWithWindows;
            ShowTooltips = SettingsManager.Instance.Settings.ShowTooltips;
            DefaultSaveLocation = SettingsManager.Instance.Settings.DefaultSaveLocation;
            FileFormat = SettingsManager.Instance.Settings.FileFormat;
        }

        private void CloseSettings()
        {
            // Navigate back to the main view
            // This will be handled by the View
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ReturnToMain();
            }
        }

        private void RecordHotkey()
        {
            if (IsRecordingHotKey) return;

            IsRecordingHotKey = true;
            RecordingHotkey = "Press any key...";
            RecordingHotkeyBackground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            RecordHotkeyButtonContent = "Stop";
        }

        private void RecordPlaybackHotkey()
        {
            if (IsRecordingHotKey) return;

            IsRecordingHotKey = true;
            PlaybackHotkey = "Press any key...";
            PlaybackHotkeyBackground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            RecordPlaybackHotkeyButtonContent = "Stop";
        }

        private void BrowseSaveLocation()
        {
            string selectedPath = _fileService.BrowseSaveLocation();
            if (!string.IsNullOrEmpty(selectedPath))
            {
                DefaultSaveLocation = selectedPath;
                SettingsManager.Instance.Settings.DefaultSaveLocation = selectedPath;
            }
        }

        private async void SaveSettingsAsync()
        {
            // Visual feedback - begin
            IsButtonEnabled = false;
            SaveButtonContent = "Saving...";

            // Save settings
            await Task.Run(() => SettingsManager.Instance.SaveSettings());

            // Visual feedback - complete
            SaveConfirmationVisibility = Visibility.Visible;
            SaveButtonContent = "Saved!";

            // Reset after delay
            await Task.Delay(1000);
            IsButtonEnabled = true;
            SaveButtonContent = "Save Settings";
            SaveConfirmationVisibility = Visibility.Collapsed;
        }
        #endregion
    }
}
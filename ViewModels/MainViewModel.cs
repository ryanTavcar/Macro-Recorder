using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Names.Models;
using Names.Services;
using Names.MacroStrategies;
using Names.Views;
using WindowsInput;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Names.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Services
        private readonly MacroRecorderService _recorderService;
        private readonly FileService _fileService;
        #endregion

        #region Private Fields
        private bool _isEditingMacroName = false;
        private string _consoleText = string.Empty;
        private int _loopCount = 1;
        private string _triggerKey = string.Empty;
        private bool _waitForTrigger = false;
        private bool _randomizeTimingCheckBox;
        private MacroSequence _currentMacroSequence = null;
        private string _macroSequenceName;
        private bool _isRecording = false;
        private bool _isPlaying = false;
        private bool _isRecordingTriggerKey = false;
        private string _triggerKeyButtonText = "Set";
        private bool _isSettingsPageVisible = false;
        private object _currentSettingsPage = null;
        private IMacroExecutionStrategy _currentExecutionStrategy;
        private CancellationTokenSource _cancellationTokenSource;
        private SolidColorBrush _statusColor = new SolidColorBrush(Colors.Blue);
        private string _statusText = "Ready";
        private string _eventsCountText = "Events: 0";
        private string _durationText = "Duration: 0s";
        private string _loopsText = "Loops: 1/1";
        private ObservableCollection<MacroSequence> _savedMacros = new ObservableCollection<MacroSequence>();
        #endregion

        #region Public Properties
        public ObservableCollection<MacroCommandViewModel> MacroCommands { get; } = new ObservableCollection<MacroCommandViewModel>();

        public MacroSequence CurrentMacroSequence
        {
            get => _currentMacroSequence;
            set => SetProperty(ref _currentMacroSequence, value);
        }
        public bool IsEditingMacroName
        {
            get => _isEditingMacroName;
            set => SetProperty(ref _isEditingMacroName, value);
        }

        public string ConsoleText
        {
            get => _consoleText;
            set => SetProperty(ref _consoleText, value);
        }

        public int LoopCount
        {
            get => _loopCount;
            set
            {
                if (SetProperty(ref _loopCount, Math.Max(1, value)))
                {
                    UpdateStatusInformation();
                }
            }
        }

        public string TriggerKey
        {
            get => _triggerKey;
            set => SetProperty(ref _triggerKey, value);
        }

        public bool WaitForTrigger
        {
            get => _waitForTrigger;
            set => SetProperty(ref _waitForTrigger, value);
        }

        public bool RandomizeTimingCheckBox
        {
            get => _randomizeTimingCheckBox;
            set
            {
                if (SetProperty(ref _randomizeTimingCheckBox, value))
                {
                    // Save the setting when it changes
                    SettingsManager.Instance.Settings.RandomizeTiming = value;
                    SettingsManager.Instance.SaveSettings();
                }
            }
        }

        public string MacroSequenceName
        {
            get => _macroSequenceName;
            set => SetProperty(ref _macroSequenceName, value);
        }

        public bool IsRecording
        {
            get => _isRecording;
            private set => SetProperty(ref _isRecording, value);
        }

        public bool IsNotRecording => !IsRecording;

        public bool IsPlaying
        {
            get => _isPlaying;
            private set => SetProperty(ref _isPlaying, value);
        }

        public string TriggerKeyButtonText
        {
            get => _triggerKeyButtonText;
            private set => SetProperty(ref _triggerKeyButtonText, value);
        }

        public bool IsSettingsPageVisible
        {
            get => _isSettingsPageVisible;
            set => SetProperty(ref _isSettingsPageVisible, value);
        }

        // Update the property to include a public set accessor
        public object CurrentSettingsPage
        {
            get => _currentSettingsPage;
            set => SetProperty(ref _currentSettingsPage, value);
        }

        public SolidColorBrush StatusColor
        {
            get => _statusColor;
            private set => SetProperty(ref _statusColor, value);
        }

        public string StatusText
        {
            get => _statusText;
            private set => SetProperty(ref _statusText, value);
        }

        public string EventsCountText
        {
            get => _eventsCountText;
            private set => SetProperty(ref _eventsCountText, value);
        }

        public string DurationText
        {
            get => _durationText;
            private set => SetProperty(ref _durationText, value);
        }

        public string LoopsText
        {
            get => _loopsText;
            private set => SetProperty(ref _loopsText, value);
        }

        public ObservableCollection<MacroSequence> SavedMacros
        {
            get => _savedMacros;
            private set => SetProperty(ref _savedMacros, value);
        }
        #endregion

        #region Commands
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
        public ICommand SaveMacroCommand { get; }
        public ICommand LoadMacroCommand { get; }
        public ICommand ClearMacroCommand { get; }
        public ICommand PlayMacroCommand { get; }
        public ICommand TogglePlaybackCommand { get; }
        public ICommand EditMacroCommand { get; }
        public ICommand DeleteMacroFromListCommand { get; }
        public ICommand DeleteCommandCommand { get; }
        public ICommand RecordTriggerKeyCommand { get; }
        public ICommand IncrementLoopCommand { get; }
        public ICommand DecrementLoopCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        //public ICommand CloseSettingsCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }
        public ICommand CloseWindowCommand { get; }
        #endregion

        public MainViewModel()
        {
            _recorderService = new MacroRecorderService();
            _fileService = new FileService();

            // Initialize commands
            StartRecordingCommand = new RelayCommand(_ => StartRecording(), _ => !IsRecording);
            StopRecordingCommand = new RelayCommand(_ => StopRecording(), _ => IsRecording);
            SaveMacroCommand = new RelayCommand(_ => SaveMacro(), _ => !IsRecording && MacroCommands.Count > 0);
            LoadMacroCommand = new RelayCommand(_ => LoadMacro(), _ => !IsRecording);
            ClearMacroCommand = new RelayCommand(_ => ClearMacro(), _ => !IsRecording);
            PlayMacroCommand = new RelayCommand(_ => PlayMacro(), _ => MacroCommands.Count > 0 && !IsRecording);
            TogglePlaybackCommand = new RelayCommand(_ => TogglePlayback(), _ => MacroCommands.Count > 0 && !IsRecording);
            EditMacroCommand = new RelayCommand(param => EditMacro(param as string), _ => !IsRecording);
            DeleteMacroFromListCommand = new RelayCommand(param => DeleteMacroFromList(param as string), _ => !IsRecording);
            DeleteCommandCommand = new RelayCommand(param => DeleteCommand(param as MacroCommandViewModel), _ => !IsRecording && !IsPlaying);
            RecordTriggerKeyCommand = new RelayCommand(_ => RecordTriggerKey());
            IncrementLoopCommand = new RelayCommand(_ => IncrementLoop());
            DecrementLoopCommand = new RelayCommand(_ => DecrementLoop(), _ => LoopCount > 1);
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            //CloseSettingsCommand = new RelayCommand(_ => CloseSettings());
            MinimizeWindowCommand = new RelayCommand(_ => MinimizeWindow());
            MaximizeWindowCommand = new RelayCommand(_ => MaximizeWindow());
            CloseWindowCommand = new RelayCommand(_ => CloseWindow());

            // Initialize the macro sequence and name
            CurrentMacroSequence = CreateMacroSequence();
            MacroSequenceName = CurrentMacroSequence.Name;

            // Subscribe to recorder service events
            _recorderService.CommandRecorded += OnCommandRecorded;

            // Load initial settings
            RandomizeTimingCheckBox = SettingsManager.Instance.Settings.RandomizeTiming;

            // Set the trigger key from user settings upon edit click
            TriggerKey = SettingsManager.Instance.Settings.PlaybackHotkey;
            if (TriggerKey != null)
            {
                WaitForTrigger = true;
            }

            // Load saved macros
            LoadSavedMacrosList();

            WriteToConsole("Macro Recorder initialized");
        }

        #region Public Methods

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (_isRecordingTriggerKey)
            {
                // Capture the trigger key
                string hotkey = e.Key.ToString();
                TriggerKey = hotkey;
                _isRecordingTriggerKey = false;
                TriggerKeyButtonText = "Set";

                SettingsManager.Instance.Settings.PlaybackHotkey = hotkey;
                SettingsManager.Instance.SaveSettings();
                
                WriteToConsole($"Trigger key set to: {TriggerKey}");
                e.Handled = true;
                return;
            }

            if (IsRecording)
            {
                _recorderService.RecordKeyPress(e.Key);
                e.Handled = true;
            }
        }
        
        public void HandleMouseDown(MouseEventArgs e)
        {
            if (IsRecording)
            {
                _recorderService.RecordMousePress(e.Source);
                e.Handled = true;
            }
        }

        public void StopMacro()
        {
            if (_currentExecutionStrategy != null)
            {
                WriteToConsole("Stopping macro execution...");
                _currentExecutionStrategy.Stop();
                _currentExecutionStrategy = null;
            }

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            IsPlaying = false;
            StatusText = "Paused";
            StatusColor = new SolidColorBrush(Colors.Blue);
        }
        
        public void UpdateStatusInformation()
        {
            int commandCount = MacroCommands.Count;
            EventsCountText = $"Events: {commandCount}";

            // Calculate total duration
            int totalDuration = MacroCommands.Sum(cmd => cmd.DelayMs);
            DurationText = $"Duration: {totalDuration / 1000.0:F3}s";

            LoopsText = $"Loops: {LoopCount}/{LoopCount}";
        }


        #endregion

        #region Private Methods
        private void LoadSavedMacrosList()
        {
            var macros = _fileService.LoadSavedMacroList();
            SavedMacros.Clear();

            foreach (var macro in macros.OrderByDescending(m => m.LastModified).Take(10))
            {
                SavedMacros.Add(macro);
            }
        }

        private void StartRecording()
        {
            IsRecording = true;
            _recorderService.StartRecording();
            WriteToConsole("Recording started...");
            StatusText = "Recording";
            StatusColor = new SolidColorBrush(Colors.Red);
        }

        private void StopRecording()
        {
            _recorderService.StopRecording();
            IsRecording = false;
            WriteToConsole("Recording stopped");
            StatusText = "Ready";
            StatusColor = new SolidColorBrush(Colors.Blue);
        }

        private void SaveMacro()
        {
            try
            {
                var sequence = CreateMacroSequence();
                sequence.Name = MacroSequenceName;
                if (sequence.Commands.Count == 0)
                {
                    MessageBox.Show("There are no macro commands to save.", "Nothing to Save",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Save the macro
                MacroSequence savedSequence = _fileService.SaveMacro(sequence);

                if (savedSequence != null)
                {
                    MacroSequenceName = savedSequence.Name;
                    WriteToConsole($"Macro saved to {_fileService.LastFilePath}");

                    MessageBox.Show($"Macro saved to {_fileService.LastFilePath}", "Save Successful",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Add the saved sequence to the sequence macrodata list
                    _fileService.SaveMacroToList(savedSequence);

                    CurrentMacroSequence = savedSequence;
                }


                // Update the saved macros list
                LoadSavedMacrosList();
            }
            catch (Exception ex)
            {
                WriteToConsole($"Save Failed: {ex.Message}");
                MessageBox.Show($"Error saving file: {ex.Message}", "Save Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMacro()
        {
            try
            {
                ClearMacro();
                var sequence = _fileService.LoadMacro();
                if (sequence != null)
                {
                    MacroSequenceName = sequence.Name;
                    foreach (var command in sequence.Commands)
                    {
                        var commandModel = new MacroCommandViewModel(command);
                        MacroCommands.Add(commandModel);
                    }
                    WriteToConsole($"Loaded {sequence.Commands.Count} macro commands from {_fileService.LastFilePath}");
                    UpdateStatusInformation();
                }
            }
            catch (Exception ex)
            {
                WriteToConsole($"Load Failed: {ex.Message}");
                MessageBox.Show($"Error loading file: {ex.Message}", "Load Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditMacro(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    ClearMacro();
                    var sequence = MacroSequence.FromFile(filePath);
                    if (sequence != null)
                    {
                        MacroSequenceName = sequence.Name;
                        CurrentMacroSequence = sequence;
                        foreach (var command in sequence.Commands)
                        {
                            var commandModel = new MacroCommandViewModel(command);
                            MacroCommands.Add(commandModel);
                        }
                        WriteToConsole($"Loaded {sequence.Commands.Count} macro commands from {filePath}");

                        // Set the trigger key from user settings upon edit click
                        TriggerKey = SettingsManager.Instance.Settings.PlaybackHotkey;
                        if (TriggerKey != null)
                        {
                            WaitForTrigger = true;
                        }

                        UpdateStatusInformation();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToConsole($"Load Failed: {ex.Message}");
                MessageBox.Show($"Error loading file: {ex.Message}", "Load Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteMacroFromList(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    var sequence = MacroSequence.FromFile(filePath);
                    if (sequence != null)
                    {
                        if (_fileService.DeleteFile(filePath))
                        {
                            // If current macro is the one being deleted, clear it
                            if (sequence.Name == MacroSequenceName)
                            {
                                MacroSequenceName = "Untitled";
                                ClearMacro();
                            }

                            WriteToConsole($"Deleted macro file: {filePath}");

                            _fileService.DeleteMacroFromList(filePath);

                            // Update the saved macros list
                            LoadSavedMacrosList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToConsole($"Delete Failed: {ex.Message}");
                MessageBox.Show($"Error deleting file: {ex.Message}", "Delete Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCommand(MacroCommandViewModel command)
        {
            if (command != null && MacroCommands.Contains(command))
            {
                MacroCommands.Remove(command);
                UpdateStatusInformation();
            }
        }

        private void ClearMacro()
        {
            MacroCommands.Clear();
            CurrentMacroSequence = CreateMacroSequence();
            MacroSequenceName = CurrentMacroSequence.Name;
            WriteToConsole("Cleared all macro commands");
            UpdateStatusInformation();
        }

        private void PlayMacro()
        {
            if (MacroCommands.Count == 0)
            {
                WriteToConsole("No macro commands to execute");
                return;
            }

            // Stop any existing execution
            StopMacro();

            // Create a new cancellation token source
            _cancellationTokenSource = new CancellationTokenSource();

            // Create input simulator
            var simulator = new InputSimulator();

            // Create the appropriate execution strategy
            _currentExecutionStrategy = MacroExecutionStrategyFactory.Create(
                WaitForTrigger,
                TriggerKey,
                simulator,
                WriteToConsole);

            // Update UI
            IsPlaying = true;
            StatusText = "Running";
            StatusColor = new SolidColorBrush(Colors.Green);

            // Execute the macro
            _currentExecutionStrategy.Execute(MacroCommands, LoopCount, _cancellationTokenSource.Token);
        }

        private void TogglePlayback()
        {
            if (IsPlaying)
            {
                StopMacro();
            }
            else
            {
                PlayMacro();
            }
        }

        private void RecordTriggerKey()
        {
            if (!_isRecordingTriggerKey)
            {
                if (IsPlaying)
                {
                    StopMacro();
                }

                _isRecordingTriggerKey = true;
                TriggerKeyButtonText = "Press Key";

                WriteToConsole("Press any key to set as trigger...");
            }
        }

        private void IncrementLoop()
        {
            LoopCount++;
        }

        private void DecrementLoop()
        {
            if (LoopCount > 1)
            {
                LoopCount--;
            }
        }

        private void OpenSettings()
        {
            // Create a new SettingsPage each time
            var settingsPage = new SettingsPage
            {
                DataContext = new SettingsViewModel()
            };
    
            // Update properties
            CurrentSettingsPage = settingsPage;
            IsSettingsPageVisible = true;
        }

        private void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }

        private MacroSequence CreateMacroSequence()
        {
            var sequence = new MacroSequence();
            foreach (var cmdVM in MacroCommands)
            {
                sequence.AddCommand(new MacroCommand(cmdVM.KeyName, cmdVM.DelayMs));
            }
            return sequence;
        }

        private void OnCommandRecorded(object sender, MacroCommand e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var viewModel = new MacroCommandViewModel(e);
                MacroCommands.Add(viewModel);
                WriteToConsole($"Key recorded: {e.KeyName}");
                UpdateStatusInformation();
            });
        }
        public void WriteToConsole(string message)
        {
            // Log to the centralized service
            LoggerService.Instance.Log(message);
        }
        #endregion
    }

    public class MacroCommandViewModel : BaseViewModel
    {
        private string _keyName;
        private int _delayMs;
        private bool _isEditingDelay;

        public string KeyName
        {
            get => _keyName;
            set => SetProperty(ref _keyName, value);
        }

        public int DelayMs
        {
            get => _delayMs;
            set => SetProperty(ref _delayMs, value);
        }

        public bool IsEditingDelay
        {
            get => _isEditingDelay;
            set => SetProperty(ref _isEditingDelay, value);
        }

        public MacroCommandViewModel(MacroCommand command)
        {
            KeyName = command.KeyName;
            DelayMs = command.DelayMs;
            IsEditingDelay = false;
        }
    }
}
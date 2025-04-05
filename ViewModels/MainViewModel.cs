using System;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Names.Models;
using Names.Services;

namespace Names.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MacroRecorderService _recorderService;
        private readonly FileService _fileService;
        private string _consoleText = string.Empty;
        private bool _isRecording;

        public ObservableCollection<MacroCommandViewModel> MacroCommands { get; } = new ObservableCollection<MacroCommandViewModel>();

        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
        public ICommand SaveMacroCommand { get; }
        public ICommand LoadMacroCommand { get; }
        public ICommand ClearMacroCommand { get; }
        public ICommand PlayMacroCommand { get; }

        public string ConsoleText
        {
            get => _consoleText;
            set => SetProperty(ref _consoleText, value);
        }

        public bool IsRecording
        {
            get => _isRecording;
            private set => SetProperty(ref _isRecording, value);
        }

        public MainViewModel()
        {
            _recorderService = new MacroRecorderService();
            _fileService = new FileService();

            StartRecordingCommand = new RelayCommand(_ => StartRecording(), _ => !IsRecording);
            StopRecordingCommand = new RelayCommand(_ => StopRecording(), _ => IsRecording);
            SaveMacroCommand = new RelayCommand(_ => SaveMacro());
            LoadMacroCommand = new RelayCommand(_ => LoadMacro());
            ClearMacroCommand = new RelayCommand(_ => ClearMacro());

            // A CanExecute condition to disable the button when there's no macro
            //PlayMacroCommand = new RelayCommand(_ => ExecuteMacro(), _ => MacroCommands.Count > 0);
            PlayMacroCommand = new RelayCommand(_ => ExecuteMacro());

            // Subscribe to recorder service events
            _recorderService.RecordingStarted += OnRecordingStarted;
            _recorderService.RecordingStopped += OnRecordingStopped;
            _recorderService.CommandRecorded += OnCommandRecorded;

            WriteToConsole("Macro Recorder initialized");
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (IsRecording)
            {
                _recorderService.RecordKeyPress(e.Key);
                e.Handled = true;
            }
        }

        private void StartRecording()
        {
            _recorderService.StartRecording();
        }

        private void StopRecording()
        {
            _recorderService.StopRecording();
        }

        private void SaveMacro()
        {
            try
            {
                var sequence = CreateMacroSequence();
                if (sequence.Commands.Count == 0)
                {
                    MessageBox.Show("There are no macro commands to save.", "Nothing to Save",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                bool success = _fileService.SaveMacro(sequence);
                if (success)
                {
                    WriteToConsole($"Macro saved to {_fileService.LastFilePath}");
                    MessageBox.Show($"Macro saved to {_fileService.LastFilePath}", "Save Successful",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
                    foreach (var command in sequence.Commands)
                    {
                        var viewModel = new MacroCommandViewModel(command);
                        MacroCommands.Add(viewModel);
                    }
                    WriteToConsole($"Loaded {sequence.Commands.Count} macro commands from {_fileService.LastFilePath}");
                }
            }
            catch (Exception ex)
            {
                WriteToConsole($"Load Failed: {ex.Message}");
                MessageBox.Show($"Error loading file: {ex.Message}", "Load Failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearMacro()
        {
            MacroCommands.Clear();
            WriteToConsole("Cleared all macro commands");
        }

        // "Play" button that will execute the recorded macro
        public void ExecuteMacro()
        {
            if (MacroCommands.Count == 0)
            {
                WriteToConsole("No macro commands to execute");
                return;
            }

            WriteToConsole("Executing macro...");

            // This is where you'd implement the actual execution
            // For now, we'll just simulate it
            //foreach (var command in MacroCommands)
            //{
            //    // Wait for the specified delay
            //    Thread.Sleep(command.DelayMs);

            //    // Convert string key name to actual key
            //    if (Enum.TryParse<Keys>(command.KeyName, out Keys key))
            //    {
            //        // Simulate key press
            //        SendKeys.SendWait(ConvertToSendKeysFormat(key));
            //    }

            //    WriteToConsole($"Executed: {command.KeyName}");
            //}

            WriteToConsole("Macro execution completed");
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

        private void OnRecordingStarted(object sender, EventArgs e)
        {
            IsRecording = true;
            WriteToConsole("Recording started...");
        }

        private void OnRecordingStopped(object sender, EventArgs e)
        {
            IsRecording = false;
            WriteToConsole("Recording stopped");
        }

        private void OnCommandRecorded(object sender, MacroCommand e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var viewModel = new MacroCommandViewModel(e);
                MacroCommands.Add(viewModel);
                WriteToConsole($"Key recorded: {e.KeyName}");
            });
        }

        public void WriteToConsole(string message)
        {
            ConsoleText += $"{message}\n";
        }
    }

    public class MacroCommandViewModel : BaseViewModel
    {
        private string _keyName;
        private int _delayMs;

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

        public MacroCommandViewModel(MacroCommand command)
        {
            KeyName = command.KeyName;
            DelayMs = command.DelayMs;
        }
    }
}
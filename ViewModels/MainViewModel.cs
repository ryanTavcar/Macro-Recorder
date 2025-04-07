using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Names.Models;
using Names.Services;
using WindowsInput;
using Names.MacroStrategies;

namespace Names.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MacroRecorderService _recorderService;
        private readonly FileService _fileService;
        private string _consoleText = string.Empty;
        private bool _isRecording;
        private int _loopCount = 1;
        private string _triggerKey = string.Empty;
        private bool _waitForTrigger = false;
        private IMacroExecutionStrategy currentExecutionStrategy;
        private CancellationTokenSource cancellationTokenSource;

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

        public int LoopCount
        {
            get => _loopCount;
            set => SetProperty(ref _loopCount, Math.Max(1, value)); // Ensure at least 1 iteration
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
            PlayMacroCommand = new RelayCommand(_ => PlayMacro());

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
        
        public void HandleMouseDown(MouseEventArgs e)
        {
            if (IsRecording)
            {
                _recorderService.RecordMousePress(e.Source);
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
                        var commandModel = new MacroCommandViewModel(command);
                        MacroCommands.Add(commandModel);
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
            ConsoleText = string.Empty;
            WriteToConsole("Cleared all macro commands");
        }

        // "Play" button that will play the recorded macro
        public void PlayMacro()
        {
            if (MacroCommands.Count == 0)
            {
                WriteToConsole("No macro commands to execute");
                return;
            }

            // Stop any existing execution
            StopMacro();

            // Create a new cancellation token source
            cancellationTokenSource = new CancellationTokenSource();

            // Create input simulator
            var simulator = new InputSimulator();

            // Create the appropriate execution strategy
            currentExecutionStrategy = MacroExecutionStrategyFactory.Create(
                WaitForTrigger,
                TriggerKey,
                simulator,
                WriteToConsole);

            // Execute the macro
            currentExecutionStrategy.Execute(MacroCommands, LoopCount, cancellationTokenSource.Token);
        }

        public void StopMacro()
        {
            if (currentExecutionStrategy != null)
            {
                WriteToConsole("Stopping macro execution...");
                currentExecutionStrategy.Stop();
                currentExecutionStrategy = null;
            }

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
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
            // Log to the centralized service instead of directly to a text box
            LoggerService.Instance.Log(message);
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
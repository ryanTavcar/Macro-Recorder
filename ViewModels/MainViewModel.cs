using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Names.Models;
using Names.Services;
using WindowsInput;
using Names.MacroStrategies;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Names.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Services
        private readonly MacroRecorderService _recorderService;
        private readonly FileService _fileService;

        // Private backing fields for UI properties
        private string _consoleText = string.Empty;
        private int _loopCount = 1;
        private string _triggerKey = string.Empty;
        private bool _waitForTrigger = false;
        private bool _randomizeTimingCheckBox;
        private string _macroSequenceName = string.Empty;

        // Execution related fields
        private IMacroExecutionStrategy currentExecutionStrategy;
        private CancellationTokenSource cancellationTokenSource;

        // Macro command collection
        public ObservableCollection<MacroCommandViewModel> MacroCommands { get; } = new ObservableCollection<MacroCommandViewModel>();
        public List<MacroSequence> _recentMacroList;
        //public MacroSequence CurrentMacroSequence;

        // Commands for UI buttons
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }
        public ICommand SaveMacroCommand { get; }
        public ICommand LoadMacroCommand { get; }
        public ICommand ClearMacroCommand { get; }
        public ICommand PlayMacroCommand { get; }
        public ICommand EditMacroCommand { get; }
        public ICommand DeleteMacroCommand { get; }

        // Bindable properties for UI
        public string ConsoleText
        {
            get => _consoleText;
            set => SetProperty(ref _consoleText, value);
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
        
        public bool RandomizeTimingCheckBox
        {
            get => _randomizeTimingCheckBox;
            set => SetProperty(ref _randomizeTimingCheckBox, value);
        }

        public string MacroSequenceName
        {
            get => _macroSequenceName;
            set => SetProperty(ref _macroSequenceName, value);
        }

        public MainViewModel()
        {
            _recorderService = new MacroRecorderService();
            _fileService = new FileService();

            StartRecordingCommand = new RelayCommand(_ => StartRecording(), _ => !_recorderService.IsRecording);
            StopRecordingCommand = new RelayCommand(_ => StopRecording(), _ => _recorderService.IsRecording);
            SaveMacroCommand = new RelayCommand(_ => SaveMacro(), _ => !_recorderService.IsRecording);
            LoadMacroCommand = new RelayCommand(_ => LoadMacro());
            ClearMacroCommand = new RelayCommand(_ => ClearMacro());
            PlayMacroCommand = new RelayCommand(_ => PlayMacro(), _ => MacroCommands.Count > 0);

            // Update the EditMacroCommand initialization to pass a string argument to the EditMacro method.
            EditMacroCommand = new RelayCommand(param => EditMacro(param as string), _ => !_recorderService.IsRecording);
            DeleteMacroCommand = new RelayCommand(param => DeleteMacro(param as string), _ => !_recorderService.IsRecording);

            // Subscribe to recorder service events
            _recorderService.CommandRecorded += OnCommandRecorded;

            WriteToConsole("Macro Recorder initialized");
        }

        public List<MacroSequence> LoadSavedMacroList()
        {
            _recentMacroList = _fileService.LoadSavedMacroList();
            Debug.WriteLine($"MainViewModel LoadSavedMacroList: {JsonConvert.SerializeObject(_recentMacroList)}");
            return _recentMacroList;
        }

        private void EditMacro(string? filePath)
        {
            try
            {
                if (!String.IsNullOrEmpty(filePath))
                {
                    ClearMacro();
                    var sequence = MacroSequence.FromFile(filePath);
                    if (sequence != null)
                    {
                        MacroSequenceName = sequence.Name;
                        foreach (var command in sequence.Commands)
                        {
                            var commandModel = new MacroCommandViewModel(command);
                            MacroCommands.Add(commandModel);
                        }
                        WriteToConsole($"Loaded {sequence.Commands.Count} macro commands from {filePath}");
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

        private void DeleteMacro(string? filePath)
        {
            try
            {
                if (!String.IsNullOrEmpty(filePath))
                {
                    var sequence = MacroSequence.FromFile(filePath);
                    if (sequence != null)
                    {
                        if (_fileService.DeleteFile(filePath))
                        {
                            // macro is in view/edit, remove it from the view
                            if (sequence.Name == MacroSequenceName)
                            {
                                MacroSequenceName = "Untitled";
                                ClearMacro();
                            }
                            WriteToConsole($"Deleted macro file: {filePath}");
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

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (_recorderService.IsRecording)
            {
                _recorderService.RecordKeyPress(e.Key);
                e.Handled = true;
            }
        }        
        
        public void HandleMouseDown(MouseEventArgs e)
        {
            if (_recorderService.IsRecording)
            {
                _recorderService.RecordMousePress(e.Source);
                e.Handled = true;
            }
        }

        private void StartRecording()
        {
            _recorderService.StartRecording();
            WriteToConsole("Recording started...");
        }

        private void StopRecording()
        {
            _recorderService.StopRecording();
            WriteToConsole("Recording stopped");
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

                // Save the updated list
                MacroSequence savedSequenced = _fileService.SaveMacro(sequence, MacroSequenceName);

                if (savedSequenced != null)
                {
                    MacroSequenceName = savedSequenced.Name;
                    WriteToConsole($"Macro saved to {_fileService.LastFilePath}");
                    MessageBox.Show($"Macro saved to {_fileService.LastFilePath}", "Save Successful",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }


                var macros = LoadSavedMacroList();

                // Remove existing entry with same name if exists
                macros.RemoveAll(m => m.Name.Equals(sequence.Name, StringComparison.OrdinalIgnoreCase));

                // Add the new/updated macro
                macros.Add(sequence);

                // Save the updated list of macros
                _fileService.SaveMacroList(macros);

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
                MacroSequenceName = sequence.Name;
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
            }
        }

        private void ClearMacro()
        {
            MacroCommands.Clear();
            MacroSequenceName = "Untitled";
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
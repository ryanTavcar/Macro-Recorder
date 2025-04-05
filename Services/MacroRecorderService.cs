using System;
using System.Windows.Input;
using Names.Models;

namespace Names.Services
{
    public class MacroRecorderService
    {
        private MacroSequence currentSequence = new MacroSequence();

        public bool IsRecording { get; private set; }

        public event EventHandler<MacroCommand> CommandRecorded;
        public event EventHandler RecordingStarted;
        public event EventHandler RecordingStopped;

        public void StartRecording()
        {
            if (!IsRecording)
            {
                IsRecording = true;
                currentSequence.Clear();
                RecordingStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void StopRecording()
        {
            if (IsRecording)
            {
                IsRecording = false;
                RecordingStopped?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RecordKeyPress(Key key)
        {
            if (IsRecording)
            {
                var command = new MacroCommand(key.ToString());
                currentSequence.AddCommand(command);
                CommandRecorded?.Invoke(this, command);
            }
        }

        public MacroSequence GetRecordedSequence()
        {
            return currentSequence;
        }
    }
}
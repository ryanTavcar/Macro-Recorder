using System;
using System.Windows;
using System.Windows.Input;
using Names.Models;

namespace Names.Services
{
    public class MacroRecorderService
    {
        private MacroSequence currentSequence = new MacroSequence();
        private DateTime lastKeyTime = DateTime.Now;
        private bool _isRecording;
        public bool IsRecording
        {
            get => _isRecording;
            set => _isRecording = value;
        }

        public event EventHandler<MacroCommand> CommandRecorded;
        //public event EventHandler RecordingStarted;
        //public event EventHandler RecordingStopped;

        public void StartRecording()
        {
            if (!IsRecording)
            {
                IsRecording = true;
                currentSequence.Clear();
                lastKeyTime = DateTime.Now;
            }
        }

        public void StopRecording()
        {
            if (IsRecording)
            {
                IsRecording = false;
            }
        }
        
        public void RecordKeyPress(Key key)
        {
            if (IsRecording)
            {

                // Calculate delay since last key press
                int delayMs = 0;
                TimeSpan elapsed = DateTime.Now - lastKeyTime;
                delayMs = (int)elapsed.TotalMilliseconds;

                // Update the timestamp for the next key
                lastKeyTime = DateTime.Now;

                var command = new MacroCommand(key.ToString(), delayMs);
                currentSequence.AddCommand(command);
                CommandRecorded?.Invoke(this, command);
            }
        }        
        
        public void RecordMousePress(object key)
        {
            if (IsRecording)
            {

                // Calculate delay since last key press
                int delayMs = 0;
                TimeSpan elapsed = DateTime.Now - lastKeyTime;
                delayMs = (int)elapsed.TotalMilliseconds;

                // Update the timestamp for the next key
                lastKeyTime = DateTime.Now;

                var command = new MacroCommand(key.ToString(), delayMs);
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
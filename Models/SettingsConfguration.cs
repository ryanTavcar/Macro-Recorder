using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names.Models
{
    public class SettingsConfguration
    {
        // Recording settings
        public string RecordingHotkey { get; set; } = "Shift";
        public bool CaptureMouseEvents { get; set; } = true;
        public bool CaptureKeyboardEvents { get; set; } = true;
        public string MouseRecordingMode { get; set; } = "MovementsAndClicks";
        public int MousePrecision { get; set; } = 5;

        // Playback settings
        public string PlaybackSpeed { get; set; } = "1.0x";
        public int DefaultLoopCount { get; set; } = 1;
        public int LoopDelay { get; set; } = 1000;
        public bool RandomizeTiming { get; set; } = true;
        public string PlaybackHotkey { get; set; } = "F9";

        // UI settings
        public string Theme { get; set; } = "Dark";
        public bool ShowConsoleWindow { get; set; } = true;
        public bool MinimizeToTray { get; set; } = true;
        public bool StartWithWindows { get; set; } = false;
        public bool ShowTooltips { get; set; } = true;

        // File settings
        public string DefaultSaveLocation { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MacroRecorder");
        public bool AutoSaveRecordings { get; set; } = true;
        public string FileFormat { get; set; } = "JSON";

        // Advanced settings
        public string ErrorHandling { get; set; } = "StopOnError";
        public string LogLevel { get; set; } = "Info";
        public bool SimulationMode { get; set; } = false;
    }
}


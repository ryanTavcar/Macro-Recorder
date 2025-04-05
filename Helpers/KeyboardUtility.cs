using System;
using WindowsInput.Native;

namespace Names.Helpers
{
    public static class KeyboardUtility
    {
        /// <summary>
        /// Converts a WPF key name to the corresponding VirtualKeyCode for InputSimulator
        /// </summary>
        public static VirtualKeyCode ConvertToVirtualKeyCode(string wpfKeyName)
        {
            switch (wpfKeyName)
            {
                // Letters
                case "A": return VirtualKeyCode.VK_A;
                case "B": return VirtualKeyCode.VK_B;
                case "C": return VirtualKeyCode.VK_C;
                case "D": return VirtualKeyCode.VK_D;
                case "E": return VirtualKeyCode.VK_E;
                case "F": return VirtualKeyCode.VK_F;
                case "G": return VirtualKeyCode.VK_G;
                case "H": return VirtualKeyCode.VK_H;
                case "I": return VirtualKeyCode.VK_I;
                case "J": return VirtualKeyCode.VK_J;
                case "K": return VirtualKeyCode.VK_K;
                case "L": return VirtualKeyCode.VK_L;
                case "M": return VirtualKeyCode.VK_M;
                case "N": return VirtualKeyCode.VK_N;
                case "O": return VirtualKeyCode.VK_O;
                case "P": return VirtualKeyCode.VK_P;
                case "Q": return VirtualKeyCode.VK_Q;
                case "R": return VirtualKeyCode.VK_R;
                case "S": return VirtualKeyCode.VK_S;
                case "T": return VirtualKeyCode.VK_T;
                case "U": return VirtualKeyCode.VK_U;
                case "V": return VirtualKeyCode.VK_V;
                case "W": return VirtualKeyCode.VK_W;
                case "X": return VirtualKeyCode.VK_X;
                case "Y": return VirtualKeyCode.VK_Y;
                case "Z": return VirtualKeyCode.VK_Z;

                // Numbers (both number row and numpad)
                case "D0":
                case "NumPad0": return VirtualKeyCode.VK_0;
                case "D1":
                case "NumPad1": return VirtualKeyCode.VK_1;
                case "D2":
                case "NumPad2": return VirtualKeyCode.VK_2;
                case "D3":
                case "NumPad3": return VirtualKeyCode.VK_3;
                case "D4":
                case "NumPad4": return VirtualKeyCode.VK_4;
                case "D5":
                case "NumPad5": return VirtualKeyCode.VK_5;
                case "D6":
                case "NumPad6": return VirtualKeyCode.VK_6;
                case "D7":
                case "NumPad7": return VirtualKeyCode.VK_7;
                case "D8":
                case "NumPad8": return VirtualKeyCode.VK_8;
                case "D9":
                case "NumPad9": return VirtualKeyCode.VK_9;

                // Function keys
                case "F1": return VirtualKeyCode.F1;
                case "F2": return VirtualKeyCode.F2;
                case "F3": return VirtualKeyCode.F3;
                case "F4": return VirtualKeyCode.F4;
                case "F5": return VirtualKeyCode.F5;
                case "F6": return VirtualKeyCode.F6;
                case "F7": return VirtualKeyCode.F7;
                case "F8": return VirtualKeyCode.F8;
                case "F9": return VirtualKeyCode.F9;
                case "F10": return VirtualKeyCode.F10;
                case "F11": return VirtualKeyCode.F11;
                case "F12": return VirtualKeyCode.F12;

                // Navigation keys
                case "Up": return VirtualKeyCode.UP;
                case "Down": return VirtualKeyCode.DOWN;
                case "Left": return VirtualKeyCode.LEFT;
                case "Right": return VirtualKeyCode.RIGHT;
                case "Home": return VirtualKeyCode.HOME;
                case "End": return VirtualKeyCode.END;
                case "PageUp": return VirtualKeyCode.PRIOR;
                case "PageDown": return VirtualKeyCode.NEXT;
                case "Insert": return VirtualKeyCode.INSERT;
                case "Delete": return VirtualKeyCode.DELETE;

                // Common keys
                case "Return": return VirtualKeyCode.RETURN;
                case "Enter": return VirtualKeyCode.RETURN;
                case "Space": return VirtualKeyCode.SPACE;
                case "Back": return VirtualKeyCode.BACK;
                case "Escape": return VirtualKeyCode.ESCAPE;
                case "Tab": return VirtualKeyCode.TAB;
                case "Capital":
                case "CapsLock": return VirtualKeyCode.CAPITAL;

                // Special characters
                case "OemTilde": return VirtualKeyCode.OEM_3; // `~
                case "OemMinus": return VirtualKeyCode.OEM_MINUS; // -_
                case "OemPlus": return VirtualKeyCode.OEM_PLUS; // =+
                case "OemOpenBrackets": return VirtualKeyCode.OEM_4; // [{
                case "OemCloseBrackets": return VirtualKeyCode.OEM_6; // ]}
                case "OemPipe": return VirtualKeyCode.OEM_5; // \|
                case "OemSemicolon": return VirtualKeyCode.OEM_1; // ;:
                case "OemQuotes": return VirtualKeyCode.OEM_7; // '"
                case "OemComma": return VirtualKeyCode.OEM_COMMA; // ,
                case "OemPeriod": return VirtualKeyCode.OEM_PERIOD; // .>
                case "OemQuestion": return VirtualKeyCode.OEM_2; // /?

                // Modifier keys
                case "LeftShift":
                case "RightShift": return VirtualKeyCode.SHIFT;
                case "LeftCtrl":
                case "RightCtrl": return VirtualKeyCode.CONTROL;
                case "LeftAlt":
                case "RightAlt": return VirtualKeyCode.MENU;
                case "LWin":
                case "RWin": return VirtualKeyCode.LWIN;

                // Media/browser keys
                case "BrowserBack": return VirtualKeyCode.BROWSER_BACK;
                case "BrowserForward": return VirtualKeyCode.BROWSER_FORWARD;
                case "BrowserRefresh": return VirtualKeyCode.BROWSER_REFRESH;
                case "VolumeMute": return VirtualKeyCode.VOLUME_MUTE;
                case "VolumeDown": return VirtualKeyCode.VOLUME_DOWN;
                case "VolumeUp": return VirtualKeyCode.VOLUME_UP;
                case "MediaNextTrack": return VirtualKeyCode.MEDIA_NEXT_TRACK;
                case "MediaPreviousTrack": return VirtualKeyCode.MEDIA_PREV_TRACK;
                case "MediaStop": return VirtualKeyCode.MEDIA_STOP;
                case "MediaPlayPause": return VirtualKeyCode.MEDIA_PLAY_PAUSE;

                default:
                    // Try to handle any letter not explicitly listed
                    if (wpfKeyName.Length == 1 && char.IsLetter(wpfKeyName[0]))
                        return (VirtualKeyCode)((int)VirtualKeyCode.VK_A + char.ToUpper(wpfKeyName[0]) - 'A');

                    // Try to handle numeric digits not explicitly listed
                    if (wpfKeyName.Length == 1 && char.IsDigit(wpfKeyName[0]))
                        return (VirtualKeyCode)((int)VirtualKeyCode.VK_0 + wpfKeyName[0] - '0');

                    // Log and throw for unsupported keys
                    Console.WriteLine($"Unsupported key: {wpfKeyName}");
                    throw new NotSupportedException($"Key {wpfKeyName} is not supported for simulation");
            }
        }
    }
}
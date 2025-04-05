using System;
using System.Windows.Input;

namespace Names.Models
{
    public class MacroCommand
    {
        public string KeyName { get; set; }
        public int DelayMs { get; set; }

        public MacroCommand(string keyName, int delayMs = 0)
        {
            KeyName = keyName;
            DelayMs = delayMs;
        }

        public override string ToString()
        {
            return $"{KeyName},{DelayMs}";
        }

        public static MacroCommand FromString(string commandString)
        {
            string[] parts = commandString.Split(',');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int delay))
            {
                return new MacroCommand(parts[0].Trim(), delay);
            }
            return new MacroCommand(parts[0].Trim());
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Names.Models
{
    public class MacroSequence
    {
        public List<MacroCommand> Commands { get; private set; } = new List<MacroCommand>();

        public void AddCommand(MacroCommand command)
        {
            Commands.Add(command);
        }

        public void Clear()
        {
            Commands.Clear();
        }

        public static MacroSequence FromFile(string filePath)
        {
            MacroSequence sequence = new MacroSequence();

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        sequence.AddCommand(MacroCommand.FromString(line));
                    }
                }
            }

            return sequence;
        }

        public void SaveToFile(string filePath)
        {
            List<string> lines = Commands.Select(cmd => cmd.ToString()).ToList();
            File.WriteAllLines(filePath, lines);
        }
    }
}
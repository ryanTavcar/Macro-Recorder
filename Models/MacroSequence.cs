using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Names.Models
{
    public class MacroSequence
    {
        public List<MacroCommand> Commands { get; private set; } = new List<MacroCommand>();
        public string Name { get; set; } = "Untitled Macro";
        public string FilePath { get; set; }
        public DateTime LastModified { get; set; }


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
            sequence.Name = Path.GetFileNameWithoutExtension(filePath);
            sequence.FilePath = filePath;
            sequence.LastModified = File.GetLastWriteTime(filePath);

            Debug.WriteLine($"HERE");


            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Debug.WriteLine($"line: {MacroCommand.FromString(line)}");
                        sequence.AddCommand(MacroCommand.FromString(line));
                    }
                }
            }

            Debug.WriteLine($"MacroSequence FromFile: {JsonConvert.SerializeObject(sequence)}");
            return sequence;
        }


        public void SaveToFile(string filePath)
        {
            List<string> lines = Commands.Select(cmd => cmd.ToString()).ToList();
            File.WriteAllLines(filePath, lines);
        }
    }
}

// Simple class to store metadata without loading full sequences
class MacroMetadata
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public DateTime LastModified { get; set; }
}
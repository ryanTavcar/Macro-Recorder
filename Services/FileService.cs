using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Names.Models;
using Newtonsoft.Json;

namespace Names.Services
{
    public class FileService
    {
        private string lastFilePath = string.Empty;
        public string LastFilePath => lastFilePath;

        public MacroSequence LoadMacro()
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Macro files (*.macro)|*.macro|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "macro",
                InitialDirectory = string.IsNullOrEmpty(lastFilePath) ?
                                 Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MacroRecorder") :
                                 Path.GetDirectoryName(lastFilePath)
            };

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    lastFilePath = openDialog.FileName;
                    return MacroSequence.FromFile(openDialog.FileName);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error loading file: {ex.Message}", ex);
                }
            }

            return null;
        }

        public bool SaveMacro(MacroSequence sequence)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Macro files (*.macro)|*.macro|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "macro",
                FileName = string.IsNullOrEmpty(lastFilePath) ? "my_macro.macro" :
                           Path.GetFileName(lastFilePath),
                InitialDirectory = string.IsNullOrEmpty(lastFilePath) ?
                                 Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MacroRecorder") :
                                 Path.GetDirectoryName(lastFilePath)
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    sequence.Name = Path.GetFileNameWithoutExtension(saveDialog.FileName);
                    sequence.FilePath = saveDialog.FileName;
                    sequence.SaveToFile(saveDialog.FileName);
                    lastFilePath = saveDialog.FileName;
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return false;
        }

        public List<MacroSequence> LoadSavedMacroList()
        {
            string metadataFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MacroRecorder",
                "macro_list.json");

            List<MacroMetadata> macroMetadataList = new List<MacroMetadata>();

            if (File.Exists(metadataFilePath))
            {
                try
                {
                    string json = File.ReadAllText(metadataFilePath);
                    macroMetadataList = JsonConvert.DeserializeObject<List<MacroMetadata>>(json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading macro list: {ex.Message}");
                    return new List<MacroSequence>();
                }
            }

            // Convert metadata to actual MacroSequence objects
            List<MacroSequence> sequences = new List<MacroSequence>();

            foreach (var metadata in macroMetadataList)
            {
                Debug.WriteLine($"DOES FILE EXIST: {File.Exists(metadata.FilePath)}");

                if (File.Exists(metadata.FilePath))
                {
                    try
                    {
                        MacroSequence sequence = MacroSequence.FromFile(metadata.FilePath);

                        sequences.Add(sequence);

                        Debug.WriteLine($"Loaded macro: {JsonConvert.SerializeObject(sequences)}");

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading macro {metadata.Name}: {ex.Message}");
                    }
                }
            }

            Debug.WriteLine($"sequences: {JsonConvert.SerializeObject(sequences)}");
            return sequences;
        }
        public void SaveMacroList(List<MacroSequence> sequences)
        {
            string directoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MacroRecorder");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string metadataFilePath = Path.Combine(directoryPath, "macro_list.json");

            // Create metadata objects from sequences
            List<MacroMetadata> metadataList = sequences.Select(s => new MacroMetadata
            {
                Name = s.Name ?? "Untitled",
                FilePath = s.FilePath,
                LastModified = DateTime.Now,
            }).ToList();

            // Save to JSON
            string json = JsonConvert.SerializeObject(metadataList, Formatting.Indented);

            Debug.WriteLine($"JSON: {json}");

            File.WriteAllText(metadataFilePath, json);
        }

        public string BrowseSaveLocation()
        {
            string? initialDirectory = string.IsNullOrEmpty(lastFilePath) ?
                                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
                                 Path.GetDirectoryName(lastFilePath);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(initialDirectory);

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Default Save Location",
                InitialDirectory = initialDirectory,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    return dialog.FileName;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return string.Empty;
        }
    }
}
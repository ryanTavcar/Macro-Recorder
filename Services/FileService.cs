using System;
using System.IO;
using Microsoft.Win32;
using Names.Models;

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
                                 Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
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
                                 Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
                                 Path.GetDirectoryName(lastFilePath)
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
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

        // Change return type to SettingsConfig when I've written it.
        public void LoadSettings()
        { 
        }

        public bool SaveSettings()
        {
            return true;
        }

    }
}
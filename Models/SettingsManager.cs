using Names.Services;
using Newtonsoft.Json;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
namespace Names.Models
{
    internal class SettingsManager
    {
        private static SettingsManager _instance;
        private static readonly object _lock = new object();

        private SettingsConfguration _currentSettings;
        private readonly string _configFilePath;

        // Singleton access point
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SettingsManager();
                        }
                    }
                }
                return _instance;
            }
        }

        // Current settings - publicly accessible
        public SettingsConfguration Settings => _currentSettings;

        // Private constructor
        private SettingsManager()
        {
            _configFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MacroRecorder",
                "settings.json");

            // Load settings on instantiation
            _currentSettings = LoadSettings();
        }

        // Load settings from file
        private SettingsConfguration LoadSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath));

                if (File.Exists(_configFilePath))
                {
                    string json = File.ReadAllText(_configFilePath);

                    // First try direct deserialization with specific settings
                    var jsonSettings = new JsonSerializerSettings
                    {
                        Culture = System.Globalization.CultureInfo.InvariantCulture,
                        FloatParseHandling = FloatParseHandling.Double
                    };

                    var config = JsonConvert.DeserializeObject<SettingsConfguration>(json, jsonSettings);
                    return config ?? new SettingsConfguration();
                }
            }
            catch (Exception ex)
            {
                LoggerService.Instance.Log($"Error loading settings: {ex.Message}");
            }

            return new SettingsConfguration();
        }
        // Save current settings to file
        public async Task<bool> SaveSettings()
        {
            return await Task.Run(() => {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath));
                    string json = JsonConvert.SerializeObject(_currentSettings, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(_configFilePath, json);
                    LoggerService.Instance.Log("Settings saved successfully");
                    return true;
                }
                catch (Exception ex)
                {
                    LoggerService.Instance.Log($"Error saving settings: {ex.Message}");
                    return false;
                }
            });
        }

        // Force reload settings from file because double types are having trouble loading correctly
        // and force reloading in SettingsPage.xaml.cs seems to fix the issue
        public void ForceReloadSettings()
        {
            _currentSettings = LoadSettings();
        }

        // You can add methods to update specific settings
        public void UpdateSetting<T>(Expression<Func<SettingsConfguration, T>> propertySelector, T value)
        {
            // This uses expression trees to get and set properties by name
            var memberExpr = (MemberExpression)propertySelector.Body;
            var property = (PropertyInfo)memberExpr.Member;

            property.SetValue(_currentSettings, value);

            // Optionally auto-save after each change
            // SaveSettings();
        }
    }
}

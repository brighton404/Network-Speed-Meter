using System;
using System.IO;
using System.Text.Json;

namespace TaskbarSpeedMeter
{
    public class UserSettings
    {
        public double LastLeft { get; set; }
        public double LastTop { get; set; }

        private static string SettingsPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user-settings.json");

        public static UserSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
                }
            }
            catch { /* ignore errors */ }
            return new UserSettings();
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch { /* ignore errors */ }
        }
    }
}

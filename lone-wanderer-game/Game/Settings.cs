using Microsoft.Xna.Framework;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

namespace LoneWandererGame.Settings
{
    public class SettingsData
    {
        public float Volume { get; set; } // 0 -> 1
        public Vector2 Resolution { get; set; }
        public bool Fullscreen { get; set; } // false = windowed, true = fullscreen

        public SettingsData() 
        {
            Volume = 0.5f;
            Resolution = new Vector2(1280, 720);
            Fullscreen = false;
        }
    };

    public class SettingsHandler
    {
        private const string PATH = "settings.json";

        public SettingsHandler() { }

        public SettingsData Load()
        {
            SettingsData settings;
            if (File.Exists(PATH))
            {
                var settingsDeserialized = File.ReadAllText(PATH);
                settings = JsonSerializer.Deserialize<SettingsData>(settingsDeserialized);
            }
            else
            {
                settings = new SettingsData();
                string settingsSerialized = JsonSerializer.Serialize<SettingsData>(settings);
                File.WriteAllText(PATH, settingsSerialized);
            }

            return settings;
        }

        public void Save(SettingsData settings)
        {
            string settingsSerialized = JsonSerializer.Serialize<SettingsData>(settings);
            File.WriteAllText(PATH, settingsSerialized);
        }
    }
}

using Microsoft.Xna.Framework;
using MonoGame.Extended.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace LoneWandererGame.Settings
{
    public class SettingsData
    {
        public int Volume { get; set; } // 0 -> 10
        public Vector2 Resolution { get; set; }
        public bool Fullscreen { get; set; } // false = windowed, true = fullscreen

        public SettingsData() 
        {
            Volume = 5;
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
                settings = JsonConvert.DeserializeObject<SettingsData>(settingsDeserialized,
                    new Vector2JsonConverter()
                );
            }
            else
            {
                settings = new SettingsData();
                string settingsSerialized = JsonConvert.SerializeObject(settings);
                File.WriteAllText(PATH, settingsSerialized);
            }

            return settings;
        }

        public void Save(SettingsData settings)
        {
            string settingsSerialized = JsonConvert.SerializeObject(settings);
            File.WriteAllText(PATH, settingsSerialized);
        }
    }
}

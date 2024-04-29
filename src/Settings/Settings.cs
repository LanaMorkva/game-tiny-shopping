using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace TinyShopping {

    public class Settings {
        public bool music { get; set; } = true;
        public bool soundEffects { get; set; } = true;
        public bool fullScreen { get; set; } = false;
        public string version { get; set; } = "1.0";
    }

    public class SettingsHandler {

        public Settings settings { get; protected set; }

        string _settingsPath = "tiny-shopping-settings.json";

        public SettingsHandler() {
            settings = new Settings();
            LoadSettings();

        }

        public void SaveSettings() {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonSettings = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_settingsPath,jsonSettings);
        }

        public bool LoadSettings() {
            if (!File.Exists(_settingsPath)) {
                return false;
            }
            string settingsString = File.ReadAllText(_settingsPath);
            Settings? loadedSettings = JsonSerializer.Deserialize<Settings>(settingsString);
            if (loadedSettings != null) {
                if (loadedSettings.version == settings.version) {
                    settings = loadedSettings;
                }
                return  true;
            }
            return false;
        }

        public void ToggleMusic() {
            settings.music = !settings.music;
            SaveSettings();
            ApplyMusic();
        }

        private void ApplyMusic() {
            if (settings.music) {
                MediaPlayer.Volume = 1;
            } else {
                MediaPlayer.Volume = 0;
            }

        }
        public void ToggleSoundEffects() {
            settings.soundEffects = !settings.soundEffects;
            SaveSettings();
            ApplySoundEffects();
        }

        private void ApplySoundEffects() {
            if (settings.soundEffects) {
                SoundEffect.MasterVolume = 1;
            } else {
                SoundEffect.MasterVolume = 0;
            }
        }
        public void ToggleFullScreen(GraphicsDeviceManager graphics) {
            settings.fullScreen = !settings.fullScreen;
            SaveSettings();
            ApplyFullScreen(graphics);
        }

        private void ApplyFullScreen(GraphicsDeviceManager graphics) {
            if (graphics.IsFullScreen != settings.fullScreen) {
                graphics.ToggleFullScreen();
            }

        }

        public void ApplySettings(GraphicsDeviceManager graphics) {
            ApplyMusic();
            ApplySoundEffects();
            ApplyFullScreen(graphics);
        }
    }
}
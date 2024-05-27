using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace TinyShopping {

    public class Settings {
        public bool music { get; set; } = true;
        public int musicVolume { get; set; } = 20;
        public bool soundEffects { get; set; } = true;

        public int effectsVolume { get; set; } = 20;
        public bool fullScreen { get; set; } = false;
		public bool firstLaunch {get; set; } = true;
        public string version { get; set; } = "1.2";
    }

    public class SettingsHandler {

        public Settings settings { get; protected set; }

        string _settingsPath = "tiny-shopping-settings.json";

        public SoundPlayer SoundPlayer { get; private set; }

        public SettingsHandler(SoundPlayer soundPlayer) {
            settings = new Settings();
            SoundPlayer = soundPlayer;
            LoadSettings();
            SoundPlayer.SetMusicMasterVolume(settings.musicVolume);
            SoundPlayer.SetEffectsMasterVolume(settings.effectsVolume);
        }

        public void SaveSettings() {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonSettings = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_settingsPath, jsonSettings);
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
                return true;
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
                SoundPlayer.SetMusicMasterVolume(settings.musicVolume);
                //MediaPlayer.Volume = 0.01f * (float)GetMusicVolume();
            } else {
                SoundPlayer.SetMusicMasterVolume(0);
                //MediaPlayer.Volume = 0;
            }
        }

        public int GetMusicVolume() {
            return settings.musicVolume;
        }

        public int GetEffectsVolume() {
            return settings.effectsVolume;
        }

        public void ToggleSoundEffects() {
            settings.soundEffects = !settings.soundEffects;
            SaveSettings();
            ApplySoundEffects();
        }

        public void ChangeMusicVolumeSettings(int volume) {
            settings.musicVolume = volume;
            ApplyMusic();
            SaveSettings();
        }

        public void ChangeEffectsVolumeSettings(int volume) {
            settings.effectsVolume = volume;
            ApplySoundEffects();
            SaveSettings();
        }

        public void SetFirstLaunch(bool value) {
            settings.firstLaunch = value;
            SaveSettings();
        }

        private void ApplySoundEffects() {
            if (settings.soundEffects) {
                SoundPlayer.SetEffectsMasterVolume(settings.effectsVolume);
                //SoundEffect.MasterVolume = 1;
            } else {
                SoundPlayer.SetEffectsMasterVolume(0);
                //SoundEffect.MasterVolume = 0;
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
            SaveSettings();

        }

        public void ApplySettings(GraphicsDeviceManager graphics) {
            ApplyMusic();
            ApplySoundEffects();
            ApplyFullScreen(graphics);
        }
    }
}
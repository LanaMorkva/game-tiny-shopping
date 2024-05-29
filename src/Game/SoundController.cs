using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using System;

namespace TinyShopping.Game {

    internal class SoundController {

        private SoundEffectInstance _regularSong;
        private SoundEffectInstance _regularSongFast;
        private SoundEffectInstance _finalMinuteIntroduction;
        private SoundEffectInstance _finalSecondsCountdown;

        private Scene _scene;

        private bool _isPlaying;

        private bool _finalMinuteStarted;
        private bool _finalMinuteSongs;
        private bool _finalSecondsStarted;

        private GameState _lastState;

        private PlayerInput _input;

        public SoundController(Scene scene) {
            _scene = scene;
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _input = new KeyboardInput(PlayerIndex.One, content); // TODO: Should be in Initialize
            _regularSong = content.Load<SoundEffect>("sounds/main_long_basic").CreateInstance();
            _regularSongFast = content.Load<SoundEffect>("sounds/main_long_fast").CreateInstance();
            _finalMinuteIntroduction = content.Load<SoundEffect>("sounds/final_minute_starts").CreateInstance();
            _finalSecondsCountdown = content.Load<SoundEffect>("sounds/countdown_last_seconds").CreateInstance();
            _regularSong.IsLooped = true;
            _regularSongFast.IsLooped = true;
        }

        public void UnloadContent(ContentManager content) {
            content.UnloadAsset("sounds/main_long_basic");
            content.UnloadAsset("sound/main_long_fast");
            content.UnloadAsset("sound/countdown_last_seconds");
            content.UnloadAsset("songs/drama");
            content.UnloadAsset("songs/drama_fast");
        }

        /// <summary>
        /// Updates the music
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="controller">The UI controller of the game</param>
        public void Update(GameTime gameTime, UIController controller) {

            if (_scene.gameState == GameState.Playing && _lastState == GameState.Paused) {
                // Continue music
                if (controller.GetRemainingTime() > 60) {
                    _scene.SettingsHandler.SoundPlayer.playSong(_regularSong, 0.6f);
                } else if (controller.GetRemainingTime() > 0) {
                    _scene.SettingsHandler.SoundPlayer.playSong(_regularSongFast, 0.6f);
                }
            } else if (_scene.gameState == GameState.Paused && _lastState == GameState.Playing) {
                // Stop music
                _regularSongFast.Stop();
                _regularSong.Stop();
            }

            if (_scene.gameState == GameState.Playing && !_isPlaying && _scene.SettingsHandler.settings.music) {
                _scene.SettingsHandler.SoundPlayer.playSong(_regularSong, 0.6f);
                _isPlaying = true;
            }

            if (_scene.gameState == GameState.Playing && !_finalMinuteStarted && controller.GetRemainingTime() <= 60 && _scene.SettingsHandler.settings.music) {
                _finalMinuteStarted = true;
                _scene.SettingsHandler.SoundPlayer.playSong(_finalMinuteIntroduction, 1f);
            }

            if (_scene.gameState == GameState.Playing && _finalMinuteStarted && !_finalMinuteSongs && controller.GetRemainingTime() <= 59.5f && _scene.SettingsHandler.settings.music) {
                _regularSong.Stop();
                _scene.SettingsHandler.SoundPlayer.playSong(_regularSongFast, 0.6f);
                _finalMinuteSongs = true;

            }

            if (_scene.gameState == GameState.Playing && !_finalSecondsStarted && controller.GetRemainingTime() <= 5 && _scene.SettingsHandler.settings.music) {
                _scene.SettingsHandler.SoundPlayer.playSong(_finalSecondsCountdown, 1f);
                _finalSecondsStarted = true;

            }

            // TODO: Swap song if battle is currently playing
            // Need a function to get if any ants are in battle

            if (_scene.gameState == GameState.Ended) {
                _regularSongFast.Stop();
                _regularSong.Stop();
            }

            _lastState = _scene.gameState;

        }
    }
}

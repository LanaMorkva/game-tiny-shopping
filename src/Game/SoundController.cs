using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;

namespace TinyShopping.Game {

    internal class SoundController {

        private Song _regularSong;
        private Song _regularSongFast;
        private Song _battleSong;
        private Song _battleSongFast;

        private Scene _scene;

        private bool _isPlaying;

        private bool _finalMinuteStarted;

        private PlayerInput _input;

        public SoundController(Scene scene) {
            _scene = scene;
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Stop();
            if (_scene.SettingsHandler.settings.music) {
                MediaPlayer.Volume = 1;
            }

        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _input = new KeyboardInput(PlayerIndex.One, content);
            _regularSong = content.Load<Song>("songs/basic_without_intro");
            _regularSongFast = content.Load<Song>("songs/basic_fast");
            _battleSong = content.Load<Song>("songs/drama");
            _battleSongFast = content.Load<Song>("songs/drama_fast");
        }

        /// <summary>
        /// Updates the music
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="controller">The UI controller of the game</param>
        public void Update(GameTime gameTime, UIController controller) {
            if (_scene.IsStarted && !_isPlaying && _scene.SettingsHandler.settings.music) {
                MediaPlayer.Play(_regularSong);
                _isPlaying = true;
            }

            if (_scene.IsStarted && !_finalMinuteStarted && controller.GetRemainingTime() <= 60 && _scene.SettingsHandler.settings.music) {
                _finalMinuteStarted = true;
                TimeSpan songPosition = MediaPlayer.PlayPosition * (10f/12f);
                if ((int) songPosition.TotalSeconds >= (int) _regularSongFast.Duration.TotalSeconds) {
                    songPosition = TimeSpan.Zero;
                }
                MediaPlayer.Play(_regularSongFast, songPosition);
            }

            // TODO: Swap song if battle is currently playing
            // Need a function to get if any ants are in battle

            if (_scene.IsOver) {
                MediaPlayer.Stop();
            }

        }
    }
}

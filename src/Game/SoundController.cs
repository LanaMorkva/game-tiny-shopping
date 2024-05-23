using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace TinyShopping.Game {

    internal class SoundController {

        private SoundEffectInstance _regularSong;
        private SoundEffectInstance _regularSongFast;
        //private Song _battleSong;
        //private Song _battleSongFast;

        private Scene _scene;

        private bool _isPlaying;

        private bool _finalMinuteStarted;

        private PlayerInput _input;

        public SoundController(Scene scene) {
            _scene = scene;
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _input = new KeyboardInput(PlayerIndex.One, content);
            _regularSong = content.Load<SoundEffect>("songs/basic_without_intro").CreateInstance();
            _regularSongFast = content.Load<SoundEffect>("songs/basic_fast").CreateInstance();
            _regularSong.IsLooped = true;
            _regularSong.IsLooped = true;
            //_regularSong = _regularSong.CreateInstance();
            //_regularSong = _regularSongFast.CreateInstance();
            //_battleSong = content.Load<Song>("songs/drama");
            //_battleSongFast = content.Load<Song>("songs/drama_fast");
        }

        public void UnloadContent(ContentManager content) {
            content.UnloadAsset("songs/basic_without_intro");
            content.UnloadAsset("songs/basic_fast");
            content.UnloadAsset("songs/drama");
            content.UnloadAsset("songs/drama_fast");
        }

        /// <summary>
        /// Updates the music
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="controller">The UI controller of the game</param>
        public void Update(GameTime gameTime, UIController controller) {
            if (_scene.gameState == GameState.Playing && !_isPlaying && _scene.SettingsHandler.settings.music) {
                _scene.SettingsHandler.SoundPlayer.playSong(_regularSong, 0.6f);
                _isPlaying = true;
            }

            if (_scene.gameState == GameState.Playing && !_finalMinuteStarted && controller.GetRemainingTime() <= 60 && _scene.SettingsHandler.settings.music) {
                _finalMinuteStarted = true;
                //TimeSpan songPosition = MediaPlayer.PlayPosition * (10f/12f);
                //_regularSong.State.
                //if ((int) songPosition.TotalSeconds >= (int) _regularSongFast.Duration.TotalSeconds) {
                    //songPosition = TimeSpan.Zero;
                //}
                _regularSong.Stop();
                _scene.SettingsHandler.SoundPlayer.playSong(_regularSongFast, 0.6f);
            }

            // TODO: Swap song if battle is currently playing
            // Need a function to get if any ants are in battle

            if (_scene.gameState == GameState.Ended) {
                _regularSongFast.Stop();
                _regularSong.Stop();
            }

        }
    }
}

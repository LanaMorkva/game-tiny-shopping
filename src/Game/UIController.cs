using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping.Game {

    internal class UIController {

        private static readonly int TIME_LIMIT_S = 5 * 60;

        private SpriteFont _font;

        private GraphicsDevice _device;

        private Rectangle _drawArea;

        private Texture2D _statsTexture;

        private Texture2D _antTexture;

        private Texture2D _appleTexture;

        private SplitScreenHandler _handler;

        private Scene _scene;

        private double _runtimeMs;

        private int _winner;

        public UIController(Rectangle drawArea, GraphicsDevice device, SplitScreenHandler handler, Scene scene) {
            _drawArea = drawArea;
            _device = device;
            _handler = handler;
            _runtimeMs = 0;
            _scene = scene;
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("Arial");
            _appleTexture = content.Load<Texture2D>("apple");
            _antTexture = content.Load<Texture2D>("ants/ant_texture");
            CreateStatisticsTexture();
        }

        /// <summary>
        /// Creates the statistics background texture.
        /// </summary>
        private void CreateStatisticsTexture() {
            _statsTexture = new Texture2D(_device, _drawArea.Width, _drawArea.Height);
            Color[] data = new Color[_drawArea.Width * _drawArea.Height];
            for (int i = 0; i < data.Length; i++) {
                data[i] = new Color(252, 239, 197);
            }
            _statsTexture.SetData(data);
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            if (!_scene.IsOver) {
                _runtimeMs += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            if (_runtimeMs > TIME_LIMIT_S * 1000) {
                _scene.IsOver = true;
                if (_handler.GetNumberOfFruits(0) > _handler.GetNumberOfFruits(1)) {
                    _winner = 1;
                }
                if (_handler.GetNumberOfFruits(0) < _handler.GetNumberOfFruits(1)) {
                    _winner = 2;
                }
            }
            if (_handler.GetNumberOfAnts(0) == 0 && _handler.GetNumberOfFruits(0) == 0) {
                _scene.IsOver = true;
                _winner = 1;
            }
            if (_handler.GetNumberOfAnts(1) == 0 && _handler.GetNumberOfFruits(1) == 0) {
                _scene.IsOver = true;
                _winner = 2;
            }
        }

        /// <summary>
        /// Draws the statistics and fps counter.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            DrawStatistics(batch);
            DrawRemainingTime(batch);
            if (_scene.IsOver) {
                DrawWinMessage(batch, gameTime);
            }
#if DEBUG
            int fps = (int) Math.Round((1000 / gameTime.ElapsedGameTime.TotalMilliseconds));
            batch.DrawString(_font, "FPS: " + fps.ToString(), new Vector2(0, 0), Color.Black, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
#endif
        }

        /// <summary>
        /// Writes the game over and win text to the screen.
        /// </summary>
        /// <param name="batch">The batch to use.</param>
        /// <param name="gameTime">The current game time.</param>
        private void DrawWinMessage(SpriteBatch batch, GameTime gameTime) {
            string text = "It's a draw!";
            if (_winner !=  0) {
                text = "Player " + _winner + " wins this round!";
            }
            Vector2 origin = _font.MeasureString(text) / 2;
            batch.DrawString(_font, text, new Vector2(_drawArea.Width / 2, _scene.Height / 2), Color.Black, 0, origin, 0.8f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws the tiem countdown.
        /// </summary>
        /// <param name="batch"></param>
        private void DrawRemainingTime(SpriteBatch batch) {
            int offsetTop = 35;
            int secs = TIME_LIMIT_S - (int) (_runtimeMs / 1000);
            int mins = secs / 60;
            secs %= 60;
            string minStr = (mins < 10 ? "0" : "") + mins;
            string secStr = (secs < 10 ? "0" : "") + secs;
            string time = minStr + ":" + secStr;
            if (secs < 0) {
                time = "00:00";
            }
            Vector2 origin = _font.MeasureString(time) / 2;
            batch.DrawString(_font, time, new Vector2(_drawArea.Width / 2, offsetTop), Color.Black, 0, origin, 0.8f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws the two players fruit and insect statistics.
        /// </summary>
        /// <param name="batch">The sprite batch to write to.</param>
        private void DrawStatistics(SpriteBatch batch) {
            int offset = 15;
            int textureSize = 40;
            int offsetText = 35;

            int offsetL = 0 + offset;
            int offsetR = _drawArea.Width - (offset + textureSize);

            batch.Draw(_statsTexture, new Vector2(0, 0), Color.White);

            batch.Draw(_antTexture, new Rectangle(offsetL, offset, textureSize, textureSize), Color.White);
            batch.Draw(_antTexture, new Rectangle(offsetR, offset, textureSize, textureSize), Color.White);

            offsetL += textureSize + 5;
            offsetR -= 5;
            String AntsNum1 = "x" + _handler.GetNumberOfAnts(0);
            Vector2 sizeAnts1 = _font.MeasureString(AntsNum1) / 2;
            batch.DrawString(_font, AntsNum1, new Vector2(offsetL, offsetText), Color.Black, 0, sizeAnts1, 0.8f, SpriteEffects.None, 0);

            String AntsNum2 = _handler.GetNumberOfAnts(1) + "x";
            Vector2 sizeAnts2 = _font.MeasureString(AntsNum2) / 2;
            batch.DrawString(_font, AntsNum2, new Vector2(offsetR, offsetText), Color.Black, 0, sizeAnts2, 0.8f, SpriteEffects.None, 0);

            offsetL += 20 + (int)sizeAnts1.X;
            offsetR -= (20 + (int)sizeAnts2.X + textureSize);
            batch.Draw(_appleTexture, new Rectangle(offsetL, offset, textureSize, textureSize), Color.White);
            batch.Draw(_appleTexture, new Rectangle(offsetR, offset, textureSize, textureSize), Color.White);

            offsetL += textureSize + 15;
            offsetR -= 15;
            String FruitsNum1 = "x" + _handler.GetNumberOfFruits(0);
            Vector2 sizeFruits1 = _font.MeasureString(FruitsNum1) / 2;
            batch.DrawString(_font, FruitsNum1, new Vector2(offsetL, offsetText), Color.Black, 0, sizeFruits1, 0.8f, SpriteEffects.None, 0);

            String FruitsNum2 = _handler.GetNumberOfFruits(1) + "x";
            Vector2 sizeFruits2 = _font.MeasureString(FruitsNum2) / 2;
            batch.DrawString(_font, FruitsNum2, new Vector2(offsetR, offsetText), Color.Black, 0, sizeFruits2, 0.8f, SpriteEffects.None, 0);
        }
    }
}

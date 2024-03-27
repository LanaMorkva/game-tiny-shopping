using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping.Game {

    internal class UIController {

        private SpriteFont _font;

        private GraphicsDevice _device;

        private Rectangle _drawArea;

        private Texture2D _statsTexture;

        private Texture2D _antTexture;

        private Texture2D _appleTexture;

        private InsectHandler _insectHandler;

        public UIController(Rectangle drawArea, GraphicsDevice device, InsectHandler insectHandler) {
            _drawArea = drawArea;
            _device = device;
            _insectHandler = insectHandler;
        }

        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("Arial");
            _appleTexture = content.Load<Texture2D>("apple");
            _antTexture = content.Load<Texture2D>("ant_texture");
            CreateStatisticsTexture();
        }

        public void Update(GameTime gameTime) {

        }

        /// <summary>
        /// Draws the statistics and fps counter.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            DrawStatistics(batch);
#if DEBUG
            int fps = (int) Math.Round((1000 / gameTime.ElapsedGameTime.TotalMilliseconds));
            batch.DrawString(_font, "FPS: " + fps.ToString(), new Vector2(0, 0), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
#endif
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
            String AntsNum1 = "x" + _insectHandler.GetNumberOfAnts(0);
            Vector2 sizeAnts1 = _font.MeasureString(AntsNum1) / 2;
            batch.DrawString(_font, AntsNum1, new Vector2(offsetL, offsetText), Color.Black, 0, sizeAnts1, 0.8f, SpriteEffects.None, 0);

            String AntsNum2 = _insectHandler.GetNumberOfAnts(1) + "x";
            Vector2 sizeAnts2 = _font.MeasureString(AntsNum2) / 2;
            batch.DrawString(_font, AntsNum2, new Vector2(offsetR, offsetText), Color.Black, 0, sizeAnts2, 0.8f, SpriteEffects.None, 0);

            offsetL += 20 + (int)sizeAnts1.X;
            offsetR -= (20 + (int)sizeAnts2.X + textureSize);
            batch.Draw(_appleTexture, new Rectangle(offsetL, offset, textureSize, textureSize), Color.White);
            batch.Draw(_appleTexture, new Rectangle(offsetR, offset, textureSize, textureSize), Color.White);

            offsetL += textureSize + 15;
            offsetR -= 15;
            String FruitsNum1 = "x" + _insectHandler.GetNumberOfFruits(0);
            Vector2 sizeFruits1 = _font.MeasureString(FruitsNum1) / 2;
            batch.DrawString(_font, FruitsNum1, new Vector2(offsetL, offsetText), Color.Black, 0, sizeFruits1, 0.8f, SpriteEffects.None, 0);

            String FruitsNum2 = _insectHandler.GetNumberOfFruits(1) + "x";
            Vector2 sizeFruits2 = _font.MeasureString(FruitsNum2) / 2;
            batch.DrawString(_font, FruitsNum2, new Vector2(offsetR, offsetText), Color.Black, 0, sizeFruits2, 0.8f, SpriteEffects.None, 0);
        }
    }
}

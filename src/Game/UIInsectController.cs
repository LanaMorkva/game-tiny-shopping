using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace TinyShopping.Game {

    /// <summary>
    /// Draws UI for insects, like the idle icon.
    /// </summary>
    internal class UIInsectController {

        private SplitScreenHandler _handler;

        private Texture2D _sleepIcon;

        public UIInsectController(SplitScreenHandler handler) {
            _handler = handler;
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _sleepIcon = content.Load<Texture2D>("sleep_indicator");
        }

        /// <summary>
        /// Draws the statistics and fps counter.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            IList<Insect> insects = _handler.GetAllInsects(0);
            foreach (var insect in insects) {
                if (!insect.IsWandering) {
                    continue;
                }
                DrawSleepIcon(batch, insect.Position, Constants.SLEEP_ICON_SIZE, _handler.Player1Area);
            }
        }

        private void DrawSleepIcon(SpriteBatch batch, Vector2 pos, int size, Rectangle viewArea) {
            Vector3 pos3 = WorldToScreenCoordinate(pos);
            Rectangle r = new Rectangle((int)pos3.X, (int)pos3.Y - size, size, size);
            if (r.Right > viewArea.Right) {
                r.X = viewArea.Right - r.Width;
            }
            batch.Draw(_sleepIcon, r, Color.White);
        }

        private Vector3 WorldToScreenCoordinate(Vector2 pos) {
            Vector3 pos3 = new Vector3(pos.X, pos.Y, 0);
            return (Matrix.CreateTranslation(pos3) * _handler.Camera1.GetViewMatrix()).Translation;
        }
    }
}

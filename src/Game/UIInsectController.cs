using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TinyShopping.Game {

    /// <summary>
    /// Draws UI for insects, like the idle icon.
    /// </summary>
    internal class UIInsectController {

        private SplitScreenHandler _handler;

        private Texture2D _iconBorderH;

        private Texture2D _iconBorderV;

        private Texture2D _sleepIcon;

        private int _borderSize;

        private int _borderPadding;

        public UIInsectController(SplitScreenHandler handler) {
            _handler = handler;
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _iconBorderV = content.Load<Texture2D>("icon_border_v");
            _iconBorderH = content.Load<Texture2D>("icon_border_h");
            _sleepIcon = content.Load<Texture2D>("sleep_indicator");
            _borderPadding = 24;
            _borderSize = Constants.INSECT_ICON_SIZE + _borderPadding * 2;
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
                DrawSleepIcon(batch, insect.Position, Constants.INSECT_ICON_SIZE, _handler.Player1Area);
            }
        }

        /// <summary>
        /// Draws a sleep icon at the given location or the border of the screen.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="pos">The world position to draw at.</param>
        /// <param name="size">The size of the icon.</param>
        /// <param name="viewArea">The view area of the player.</param>
        private void DrawSleepIcon(SpriteBatch batch, Vector2 pos, int size, Rectangle viewArea) {
            Vector3 pos3 = WorldToScreenCoordinate(pos);
            Rectangle borderR = new Rectangle(
                (int)(pos3.X - _borderSize / 2), 
                (int)(pos3.Y - _borderSize / 2 - 24), 
                _borderSize,
                _borderSize);
            SpriteEffects effect = SpriteEffects.None;
            Texture2D border = _iconBorderH;
            bool needBorder = false;
            if (borderR.Right > viewArea.Right) {
                needBorder = true;
                borderR.X = viewArea.Right - _borderSize;
            }
            if (borderR.Left < viewArea.Left) {
                needBorder = true;
                effect = SpriteEffects.FlipHorizontally;
                borderR.X = viewArea.Left;
            }
            if (borderR.Top < viewArea.Top) {
                needBorder = true;
                borderR.Y = viewArea.Top;
                border = _iconBorderV;
            }
            if (borderR.Bottom > viewArea.Bottom) {
                needBorder = true;
                borderR.Y = viewArea.Bottom - _borderSize - 32;
                effect = SpriteEffects.FlipVertically;
                border = _iconBorderV;
            }
            if (needBorder) {
                batch.Draw(border, borderR, null, Color.White, 0, new Vector2(), effect, 0);
            }
            Rectangle r = new Rectangle(borderR.Center.X - size / 2, borderR.Center.Y - size / 2, size, size);
            batch.Draw(_sleepIcon, r, Color.White);
        }

        /// <summary>
        /// Converts the given world position to screen coordinates.
        /// </summary>
        /// <param name="pos">The world position to convert.</param>
        /// <returns>The screen position.</returns>
        private Vector3 WorldToScreenCoordinate(Vector2 pos) {
            Vector3 pos3 = new Vector3(pos.X, pos.Y, 0);
            return (Matrix.CreateTranslation(pos3) * _handler.Camera1.GetViewMatrix()).Translation;
        }
    }
}

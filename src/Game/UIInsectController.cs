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
            _borderPadding = 10;
            _borderSize = Constants.INSECT_ICON_SIZE + _borderPadding * 2;
        }

        /// <summary>
        /// Draws icons.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            DrawSleepIconForPlayer(batch, 0, _handler.Player1Area, _handler.Camera1);
            DrawSleepIconForPlayer(batch, 1, _handler.Player2Area, _handler.Camera2);
        }

        /// <summary>
        /// Draws the sleep icon for the insects of the given player. 
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="player">The player to draw the icons for.</param>
        /// <param name="playerArea">The screen area to draw to.</param>
        /// <param name="camera">The camera to draw to.</param>
        private void DrawSleepIconForPlayer(SpriteBatch batch, int player, Rectangle playerArea, Camera2D camera) {
            IList<Insect> insects = _handler.GetAllInsects(player);
            foreach (var insect in insects) {
                if (!insect.IsWandering) {
                    continue;
                }
                DrawSleepIcon(batch, insect.Position, Constants.INSECT_ICON_SIZE, playerArea, camera);
            }
        }

        /// <summary>
        /// Draws a sleep icon at the given location or the border of the screen.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="pos">The world position to draw at.</param>
        /// <param name="size">The size of the icon.</param>
        /// <param name="viewArea">The view area of the player.</param>
        /// <param name="camera">The camera to draw to.</param>
        private void DrawSleepIcon(SpriteBatch batch, Vector2 pos, int size, Rectangle viewArea, Camera2D camera) {
            Vector3 pos3 = WorldToScreenCoordinate(pos, camera);
            Rectangle borderR = new Rectangle(
                (int)(pos3.X + viewArea.X - _borderSize / 2), 
                (int)(pos3.Y - _borderSize / 2 - 24), 
                _borderSize,
                _borderSize);
            SpriteEffects effect = SpriteEffects.None;
            Texture2D border = _iconBorderH;
            bool needBorder = false;
            if (borderR.Right > viewArea.Right) {
                needBorder = true;
                borderR.X = viewArea.Right - _borderSize;
                batch.Draw(border, borderR, null, Color.White, 0, new Vector2(), effect, 0);
                Rectangle r = new Rectangle(borderR.Center.X - size / 2 - 8, borderR.Center.Y - size / 2, size, size);
                batch.Draw(_sleepIcon, r, Color.White);
                return;
            }
            if (borderR.Left < viewArea.Left) {
                needBorder = true;
                effect = SpriteEffects.FlipHorizontally;
                borderR.X = viewArea.Left;
                batch.Draw(border, borderR, null, Color.White, 0, new Vector2(), effect, 0);
                Rectangle r = new Rectangle(borderR.Center.X - size / 2 + 8, borderR.Center.Y - size / 2, size, size);
                batch.Draw(_sleepIcon, r, Color.White);
                return;
            }
            if (borderR.Top < viewArea.Top) {
                needBorder = true;
                borderR.Y = viewArea.Top;
                border = _iconBorderV;
                batch.Draw(border, borderR, null, Color.White, 0, new Vector2(), effect, 0);
                Rectangle r = new Rectangle(borderR.Center.X - size / 2, borderR.Center.Y - size / 2 + 8, size, size);
                batch.Draw(_sleepIcon, r, Color.White);
                return;
            }
            if (borderR.Bottom > viewArea.Bottom) {
                needBorder = true;
                borderR.Y = viewArea.Bottom - _borderSize - 32;
                effect = SpriteEffects.FlipVertically;
                border = _iconBorderV;
                batch.Draw(border, borderR, null, Color.White, 0, new Vector2(), effect, 0);
                Rectangle r = new Rectangle(borderR.Center.X - size / 2, borderR.Center.Y - size / 2 - 8, size, size);
                batch.Draw(_sleepIcon, r, Color.White);
                return;
            }
            //if (needBorder) {
            //    batch.Draw(border, borderR, null, Color.White, 0, new Vector2(), effect, 0);
            //}
            Rectangle r_ = new Rectangle(borderR.Center.X - size / 2, borderR.Center.Y - size / 2, size, size);
            batch.Draw(_sleepIcon, r_, Color.White);
        }

        /// <summary>
        /// Converts the given world position to screen coordinates.
        /// </summary>
        /// <param name="pos">The world position to convert.</param>
        /// <returns>The screen position.</returns>
        private Vector3 WorldToScreenCoordinate(Vector2 pos, Camera2D camera) {
            Vector3 pos3 = new Vector3(pos.X, pos.Y, 0);
            return (Matrix.CreateTranslation(pos3) * camera.GetViewMatrix()).Translation;
        }
    }
}

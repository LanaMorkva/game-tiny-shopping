using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.MainMenu
{
    internal class Scene : TinyShopping.Scene {

        private Texture2D _background;

        private SpriteFont _font;

        private Vector2 _offset;

        private Rectangle _backgroundPosition;

        public Scene(ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager, Renderer game): 
            base(content, device, manager, game) {
        }

        public override void LoadContent() {
            _background = Content.Load<Texture2D>("teaser");
            _font = Content.Load<SpriteFont>("Arial");
            CalculateBackgroundPosition();
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            KeyboardState state = Keyboard.GetState();
            GamePadState cState = GamePad.GetState(PlayerIndex.One);
            if (state.GetPressedKeys().Length > 0 || cState.IsButtonDown(Buttons.A)) {
                Game.ChangeScene(new Game.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game));
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();
            SpriteBatch.Draw(_background, _backgroundPosition, Color.White);
            Vector2 pos = new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth/2, GraphicsDeviceManager.PreferredBackBufferHeight*5/6);
            String message = "Press any key to continue";
            Vector2 textSize = _font.MeasureString(message);
            SpriteBatch.DrawString(_font, message, pos, Color.Black, 0, textSize / 2, 2.01f, SpriteEffects.None, 0);
            SpriteBatch.DrawString(_font, message, pos, Color.White, 0, textSize/2, 2, SpriteEffects.None, 0);
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Calculates the position and size of the background such that it is fully on screen.
        /// </summary>
        private void CalculateBackgroundPosition() {
            Vector2 screen = new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight);
            float ratio;
            if (_background.Height / screen.Y > _background.Width / screen.X) {
                ratio = screen.Y / _background.Height;
            }
            else {
                ratio = screen.X / _background.Width;
            }
            int worldWidth = (int)(_background.Width * ratio);
            int worldHeight = (int)(_background.Height * ratio);
            int xOffset = (int)((screen.X - worldWidth) / 2.0);
            int yOffset = (int)((screen.Y - worldHeight) / 2.0);
            _offset = new Vector2(xOffset, yOffset);
            _backgroundPosition = new Rectangle(xOffset, yOffset, worldWidth, worldHeight);
        }
    }
}

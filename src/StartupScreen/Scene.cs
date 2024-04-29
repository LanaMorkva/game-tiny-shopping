﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TinyShopping.StartupScreen
{
    internal class Scene : TinyShopping.Scene {

        private Texture2D _background;

        private SpriteFont _font;

        private Rectangle _backgroundPosition;

        public Scene(ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler): 
            base(content, device, manager, game, settingsHandler) {
        }

        public override void LoadContent() {
            _background = Content.Load<Texture2D>("teaser");
            _font = Content.Load<SpriteFont>("fonts/General");
            CalculateBackgroundPosition();
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            KeyboardState state = Keyboard.GetState();
            GamePadState cState = GamePad.GetState(PlayerIndex.One);
            if (state.GetPressedKeys().Length > 0 || cState.IsButtonDown(Buttons.A)) {
                Game.ChangeScene(new MainMenu.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game, SettingsHandler));
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();
            SpriteBatch.Draw(_background, _backgroundPosition, Color.White);
            Vector2 pos = new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth/2, GraphicsDeviceManager.PreferredBackBufferHeight*5/6);
            string message = "PRESS ANY KEY TO CONTINUE";
            Vector2 textSize = _font.MeasureString(message) / 2;
            SpriteBatch.DrawString(_font, message, pos - new Vector2(5,5), Color.Black, 0, textSize, 0.9f, SpriteEffects.None, 0);
            SpriteBatch.DrawString(_font, message, pos, Color.White, 0, textSize, 0.9f, SpriteEffects.None, 0);
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
            _backgroundPosition = new Rectangle(xOffset, yOffset, worldWidth, worldHeight);
        }
    }
}
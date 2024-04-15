﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping.Game {

    public class Scene : TinyShopping.Scene {

        private SpriteBatch _spriteBatch;

        private UIController _ui;

        private SplitScreenHandler _splitScreenHandler;

        private Rectangle _player1Area;

        private Rectangle _player2Area;

        public bool IsStarted {get; set; }
        public bool IsOver { get; set; }

        public int Height {  get; private set; }
        public int Width { get; private set; }

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
        }

        public override void Initialize() {
            Width = GraphicsDeviceManager.PreferredBackBufferWidth;
            Height = GraphicsDeviceManager.PreferredBackBufferHeight;
            _player1Area = new Rectangle(0, 0, Width / 2, Height);
            _player2Area = new Rectangle(Width / 2, 0, Width / 2, Height);
            _splitScreenHandler = new SplitScreenHandler(_player1Area, _player2Area, GraphicsDevice);
            _splitScreenHandler.Initialize();
            _ui = new UIController(GraphicsDevice, _splitScreenHandler, this);
            base.Initialize();
        }

        public override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _splitScreenHandler.LoadContent(Content);
            _ui.LoadContent(Content);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            if (!IsOver && IsStarted) {
                _splitScreenHandler.Update(gameTime);
            }
            _ui.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            _spriteBatch.Begin();
            
            Viewport original = GraphicsDevice.Viewport;
            _splitScreenHandler.Draw(_spriteBatch, gameTime);
            _ui.Draw(_spriteBatch, gameTime);
            GraphicsDevice.Viewport = original;
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

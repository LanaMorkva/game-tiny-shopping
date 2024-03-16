using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameLab.TinyShopping {

    public class Game : Microsoft.Xna.Framework.Game {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private World _world;
        private List<Insect> _ants = new List<Insect>();

        public Game() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = 1400;
            _graphics.PreferredBackBufferHeight = 1024;
            //_graphics.ToggleFullScreen();
            _graphics.ApplyChanges();

            _world = new World(_graphics);
            for (int i = 0; i < 20; ++i) {
                _ants.Add(new Insect(_world));
            }

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _world.LoadContent(Content);
            foreach (Insect insect in _ants) {
                insect.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (Insect insect in _ants) {
                insect.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _world.Draw(_spriteBatch, gameTime);
            foreach (Insect insect in _ants) {
                insect.Draw(_spriteBatch, gameTime);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

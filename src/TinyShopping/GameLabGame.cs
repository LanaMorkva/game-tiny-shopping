using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameLab.TinyShopping {

    public class Game : Microsoft.Xna.Framework.Game {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private World _world;

        private Colony _colony1;

        private Colony _colony2;

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
            _colony1 = new Colony(new Vector2(300, 300), _world);
            _colony1.Initialize();
            _colony2 = new Colony(new Vector2(1200, 800), _world);
            _colony2.Initialize();

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _world.LoadContent(Content);
            _colony1.LoadContent(Content);
            _colony2.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _colony1.Update(gameTime);
            _colony2.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _world.Draw(_spriteBatch, gameTime);
            _colony1.Draw(_spriteBatch, gameTime);
            _colony2.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

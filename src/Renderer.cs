using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping {

    public class Renderer : Microsoft.Xna.Framework.Game {

        private GraphicsDeviceManager _graphics;
        private Scene _scene;

        private SettingsHandler _settingsHandler;

        public Renderer() {
            _graphics = new GraphicsDeviceManager(this); 
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _settingsHandler = new SettingsHandler();
        }

        protected override void Initialize() {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();

            _settingsHandler.ApplySettings(_graphics);

            _scene = new MainMenu.Scene(Content, GraphicsDevice, _graphics, this, _settingsHandler);
            _scene.Initialize();

            base.Initialize();
        }

        protected override void LoadContent() {
            _scene.LoadContent();
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime) {
            _scene.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            _scene.Draw(gameTime);
            base.Draw(gameTime);
        }

        public void ChangeScene(Scene newScene) {
            newScene.Initialize();
            newScene.LoadContent();
            _scene = newScene;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping {

    public class Renderer : Microsoft.Xna.Framework.Game {

        private GraphicsDeviceManager _graphics;
        private Scene _scene;
        private Scene _prevScene;

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
            _prevScene = null;

            base.Initialize();
        }

        protected override void LoadContent() {
            _scene.LoadContent();
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            _scene.UnloadContent();
            base.UnloadContent();
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

        public void ChangeSceneUnload(Scene newScene) {
            _prevScene = null;
            _scene.Terminate();
            _scene.UnloadContent();
            newScene.Initialize();
            newScene.LoadContent();
            _scene = newScene;
        }

        public void ChangeScene(Scene newScene) {
            _scene.Terminate();
            newScene.Initialize();
            newScene.LoadContent();
            _prevScene = _scene;
            _scene = newScene;
        }

        public void GoBack() {
            if (_prevScene != null) {
                _scene.Terminate();
                _scene.UnloadContent();
                _scene = _prevScene;
                _prevScene = null;
            }
        }
    }
}

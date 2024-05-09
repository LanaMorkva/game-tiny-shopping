using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TinyShopping {

    public abstract class Scene {

        public bool IsStarted {get; set; }
        public bool IsOver { get; set; }

        public bool IsPaused {get; set; }

        public int Height {  get; private set; }
        public int Width { get; private set; }

        protected ContentManager Content { get; private set; }

        public GraphicsDevice GraphicsDevice { get; private set; }

        protected GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        protected SpriteBatch SpriteBatch { get; private set; }

        protected Renderer Game { get; private set; }

        public SettingsHandler SettingsHandler { get; private set; }

        protected Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) {
            Content = content;
            GraphicsDevice = graphics;
            GraphicsDeviceManager = manager;
            Game = game;
            SettingsHandler = settingsHandler;

            Width = manager.PreferredBackBufferWidth;
            Height = manager.PreferredBackBufferHeight;
        }

        public virtual void Initialize() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public virtual void LoadContent() {

        }

        public virtual void UnloadContent() {

        }

        public virtual void Terminate() {
            MediaPlayer.Stop();
        }

        public virtual void Update(GameTime gameTime) {
#if DEBUG
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Tab)) {
                Game.Exit();
            }
#endif

        }

        public virtual void Draw(GameTime gameTime) {

        }

        public void LoadMainMenu() {
            Game.ChangeScene(new MainMenu.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game, SettingsHandler));
        }

        public void ResumeGame() {
            IsPaused = false;
        }
    }
}

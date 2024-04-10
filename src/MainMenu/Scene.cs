using System.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping.MainMenu {

    public class Scene : TinyShopping.Scene {


        private SpriteBatch _spriteBatch;

        private SelectMenu _selectMenu;

        public int Height {  get; private set; }
        public int Width {  get; private set; }

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game) :
            base(content, graphics, manager, game) {
        }

        public override void Initialize() {
            Height = GraphicsDeviceManager.PreferredBackBufferHeight;
            Width = GraphicsDeviceManager.PreferredBackBufferWidth;
            _selectMenu = new SelectMenu(GraphicsDevice, this);
            base.Initialize();
        }

        public override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _selectMenu.LoadContent(Content);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            _selectMenu.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {

            _spriteBatch.Begin();
            
            _selectMenu.Draw(_spriteBatch, gameTime);
            Viewport original = GraphicsDevice.Viewport;
            GraphicsDevice.Viewport = original;
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void StartGame() {
            Game.ChangeScene(new Game.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game));
        }

        public void NotImplementedScene() {
            // empty
        }

        public void ExitGame() {
            Game.Exit();
        }
    }
}

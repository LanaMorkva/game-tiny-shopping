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
<<<<<<< HEAD
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _selectMenu.LoadContent(Content);
=======
            _background = Content.Load<Texture2D>("teaser");
            _font = Content.Load<SpriteFont>("General");
            CalculateBackgroundPosition();
>>>>>>> development
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            _selectMenu.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
<<<<<<< HEAD

            _spriteBatch.Begin();
            
            _selectMenu.Draw(_spriteBatch, gameTime);
            Viewport original = GraphicsDevice.Viewport;
            GraphicsDevice.Viewport = original;
            
            _spriteBatch.End();
=======
            SpriteBatch.Begin();
            SpriteBatch.Draw(_background, _backgroundPosition, Color.White);
            Vector2 pos = new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth/2, GraphicsDeviceManager.PreferredBackBufferHeight*5/6);
            String message = "PRESS ANY KEY TO CONTINUE";
            Vector2 textSize = _font.MeasureString(message) / 2;
            SpriteBatch.DrawString(_font, message, pos - new Vector2(5,5), Color.Black, 0, textSize, 0.9f, SpriteEffects.None, 0);
            SpriteBatch.DrawString(_font, message, pos, Color.White, 0, textSize, 0.9f, SpriteEffects.None, 0);
            SpriteBatch.End();
>>>>>>> development
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

using System.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping.MainMenu
{

    public class Scene : TinyShopping.Scene
    {


        private SpriteBatch _spriteBatch;

        private SelectMenu _selectMenu;

        private Texture2D _backgroundTexture;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game) :
            base(content, graphics, manager, game)
        {
        }

        public override void Initialize()
        {
            Height = GraphicsDeviceManager.PreferredBackBufferHeight;
            Width = GraphicsDeviceManager.PreferredBackBufferWidth;
            _selectMenu = new SelectMenu(new Vector2(Width * 0.382f, Height / 2.0f), new Vector2(20, 20), 20);
            _selectMenu.AddItem("New Game", StartGame);
            _selectMenu.AddItem("How to play", NotImplementedScene);
            _selectMenu.AddItem("Settings", NotImplementedScene);
            _selectMenu.AddItem("Quit", ExitGame);
            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("teaser");
            _selectMenu.LoadContent(Content);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _selectMenu.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            _spriteBatch.Begin();

            _selectMenu.Draw(_spriteBatch, gameTime);
            Viewport original = GraphicsDevice.Viewport;
            GraphicsDevice.Viewport = original;

            // Draw menu texture
            Vector2 backgroundCenter = new((int)(Width / 1.618), (int)(Height / 2.0));
            Vector2 backgroundSize = new(500, 500);
            Vector2 origin = backgroundCenter - backgroundSize;
            _spriteBatch.Draw(_backgroundTexture, new Rectangle((int)origin.X, (int)origin.Y, 2 * (int)backgroundSize.X, 2 * (int)backgroundSize.Y), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void StartGame()
        {
            Game.ChangeScene(new Game.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game));
        }

        public void NotImplementedScene()
        {
            // empty
        }

        public void ExitGame()
        {
            Game.Exit();
        }
    }
}

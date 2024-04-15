using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.MainMenu
{

    public class Scene : TinyShopping.Scene
    {


        private SpriteBatch _spriteBatch;

        private SelectMenu _selectMenu;

        private Texture2D _titleTexture;
        private Texture2D _imageTexture;

        private Rectangle _imageRegion;
        private Rectangle _titleRegion;

        private Color _backColor = new Color(211, 237, 150);

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

            int menuPosY = (int)(Height / 10);
            int menuW = (int)(Width / 2.3);
            var menuRegion = new Rectangle(menuW / 10, menuPosY, menuW, Height - menuPosY);
            var menuItemSize = new Vector2((int)(Width / 2.8), Height / 10);

            _titleRegion = new Rectangle(menuW / 8, (int)(menuPosY / 1.5), menuW, menuPosY);
            _imageRegion = new Rectangle((int)(menuW / 1.5), menuPosY / 3, (int)(Width - menuW / 1.5),
                Height - menuPosY / 3);

            _selectMenu = new SelectMenu(menuRegion, menuItemSize);
            _selectMenu.AddItem("New Game", StartGame);
            _selectMenu.AddItem("How to play", NotImplementedScene);
            _selectMenu.AddItem("Settings", NotImplementedScene);
            _selectMenu.AddItem("Quit", ExitGame);
            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _imageTexture = Content.Load<Texture2D>("main_menu/teaser");
            _titleTexture = Content.Load<Texture2D>("main_menu/game_title");
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

            var backRegion = new RectangleF(0, 0, Width, Height);
            _spriteBatch.FillRectangle(backRegion, _backColor);
            // Draw menu texture
            _spriteBatch.Draw(_imageTexture, _imageRegion, new Rectangle(40, 70, 535, 390), Color.White);
            _spriteBatch.Draw(_titleTexture, _titleRegion, Color.White);
            _selectMenu.Draw(_spriteBatch);

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

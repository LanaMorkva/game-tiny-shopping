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
        private Vector2 _titleLocation;

        private SpriteFont _font;

        private Color _backColor = new Color(211, 237, 150);

        public int Height { get; private set; }
        public int Width { get; private set; }

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler)
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

            _imageRegion = new Rectangle((int)(menuW / 1.5), menuPosY / 3, (int)(Width - menuW / 1.5),
                Height - menuPosY / 3);
            _titleLocation = new Vector2(menuW / 8, (int)(menuPosY / 3));


            Vector2 centerOffset = new Vector2(0, -(menuRegion.Y / 3));

            _selectMenu = new MainSelectMenu(menuRegion, centerOffset, menuItemSize);
            _selectMenu.AddItem(new MainMenuItem("New Game", StartGame));
            _selectMenu.AddItem(new MainMenuItem("How to play", NotImplementedScene));
            _selectMenu.AddItem(new MainMenuItem("Settings", SettingsMenu));
            _selectMenu.AddItem(new MainMenuItem("Quit", ExitGame));
            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _imageTexture = Content.Load<Texture2D>("main_menu/teaser");
            _titleTexture = Content.Load<Texture2D>("main_menu/game_title");
            _font = Content.Load<SpriteFont>("fonts/General");
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
            // Draw title
            _spriteBatch.DrawString(_font, "Tiny Shopping", _titleLocation, Color.Coral, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            _selectMenu.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void StartGame()
        {
            Game.ChangeScene(new Game.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game, SettingsHandler));
        }

        public void SettingsMenu()
        {
            Game.ChangeScene(new SettingsMenu.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game, SettingsHandler));
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

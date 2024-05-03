using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.ControlsTutorial
{

    public class Scene : TinyShopping.Scene
    {
        private SpriteBatch _spriteBatch;

        private SelectMenu _selectMenu;

        private Texture2D _gamepadTexture;
        private Texture2D _keyboardTexture;

        private Texture2D _selectedTexture;

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

            int menuPosY = (int)(Height / 20);
            int menuW = (int)(Width / 2);
            var menuRegion = new Rectangle(-Width/3, -Height/5, Width, Height);
            var menuItemSize = new Vector2((int)(Width / 4.5), Height / 10);

            Rectangle explanationRegion = new Rectangle(50, Height - 150, 300, 100);
            _selectMenu = new SelectMenu(menuRegion, menuItemSize, LoadMainMenu, explanationRegion);
            _selectMenu.AddItem(new MainMenu.MainMenuItem("GamePad", SelectGamepadTexture));
            _selectMenu.AddItem(new MainMenu.MainMenuItem("Keyboard", SelectKeyboardTexture));
            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gamepadTexture = Content.Load<Texture2D>("tutorial/controls_gamepad");
            _keyboardTexture = Content.Load<Texture2D>("tutorial/controls_keyboard");

            _selectedTexture = _gamepadTexture;

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

            var screenRegion = new Rectangle(0, 0, Width, Height);
            _spriteBatch.FillRectangle(screenRegion, _backColor);
            _spriteBatch.Draw(_selectedTexture, screenRegion, Color.White);
            _selectMenu.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void SelectGamepadTexture() {
            _selectedTexture = _gamepadTexture;
        }

        public void SelectKeyboardTexture() {
            _selectedTexture = _keyboardTexture;
        }

        public void LoadMainMenu() {
            Game.ChangeScene(new MainMenu.Scene(Content, GraphicsDevice, GraphicsDeviceManager, Game, SettingsHandler));
        }
    }
}

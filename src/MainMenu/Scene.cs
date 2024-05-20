using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;

namespace TinyShopping.MainMenu
{

    public class Scene : TinyShopping.Scene
    {
        private SelectMenu _selectMenu;

        private Texture2D _titleTexture;
        private Texture2D _imageTexture;

        private Rectangle _imageRegion;
        private Vector2 _titleLocation;

        private SpriteFont _font;

        private Song _backgroundSong;
        private SoundEffectInstance _supermarketNoiseInstance;

        private Color _backColor = new Color(211, 237, 150);

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler)
        {
        }

        public override void Initialize()
        {
            int menuPosY = (int)(Height / 10);
            int menuW = (int)(Width / 2.3);
            var menuRegion = new Rectangle(menuW / 10, menuPosY, menuW, Height - menuPosY);
            var menuItemSize = new Vector2((int)(Width / 3.5), Height / 12);

            _imageRegion = new Rectangle((int)(menuW / 1.5), menuPosY / 3, (int)(Width - menuW / 1.5), Height - menuPosY / 3);
            _titleLocation = new Vector2(menuW / 8, (int)(menuPosY / 3));

            Vector2 centerOffset = new Vector2(0, -(menuRegion.Y / 3));
            
            Rectangle explanationRegion = new Rectangle(50, Height - 150, 300, 100);
            _selectMenu = new MainSelectMenu(menuRegion, centerOffset, menuItemSize, explanationRegion);
            _selectMenu.AddItem(new MainMenuItem("New Game", StartGame));
            _selectMenu.AddItem(new MainMenuItem("Tutorial", StartTutorial));
            _selectMenu.AddItem(new MainMenuItem("Controls", ControlsTutorial));
            _selectMenu.AddItem(new MainMenuItem("Settings", SettingsMenu));
            _selectMenu.AddItem(new MainMenuItem("Quit", ExitGame));

            base.Initialize();
        }

        public override void LoadContent()
        {
            _imageTexture = Content.Load<Texture2D>("main_menu/teaser");
            _titleTexture = Content.Load<Texture2D>("main_menu/game_title");
            _font = Content.Load<SpriteFont>("fonts/General");
            _selectMenu.LoadContent(Content);
            _backgroundSong = Content.Load<Song>("songs/basic_supermarket");
            SoundEffect supermarketNoise = Content.Load<SoundEffect>("sounds/supermarket_atmosphere");
            base.LoadContent();
            MediaPlayer.Play(_backgroundSong);
            MediaPlayer.IsRepeating = true;
            _supermarketNoiseInstance = supermarketNoise.CreateInstance();
            _supermarketNoiseInstance.IsLooped = true;
            _supermarketNoiseInstance.Play();
        }

        public override void Update(GameTime gameTime)
        {
            _selectMenu.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            SpriteBatch.Begin();

            var backRegion = new RectangleF(0, 0, Width, Height);
            SpriteBatch.FillRectangle(backRegion, _backColor);
            // Draw menu texture
            SpriteBatch.Draw(_imageTexture, _imageRegion, new Rectangle(40, 70, 535, 390), Color.White);
            // Draw title
            SpriteBatch.DrawString(_font, "Tiny Shopping", _titleLocation, Color.Coral, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            _selectMenu.Draw(SpriteBatch);

            SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Terminate()
        {
            base.Terminate();
            // If loop is not falsified for some reason this has a side effect for the MediaPlayer
            _supermarketNoiseInstance.IsLooped = false;
            _supermarketNoiseInstance.Stop(true);
        }

        public void StartGame()
        {
            Game.ChangeSceneUnload(new Game.GameScene(Content, GraphicsDevice, GraphicsDeviceManager, Game, SettingsHandler));
        }

        public void StartTutorial() 
        {
            Game.ChangeSceneUnload(new Game.TutorialScene(Content, GraphicsDevice, GraphicsDeviceManager, Game, SettingsHandler));
        }

        public void ExitGame()
        {
            Game.Exit();
        }
    }
}

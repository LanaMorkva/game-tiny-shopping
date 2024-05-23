using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.MainMenu
{

    public class Scene : TinyShopping.Scene
    {
        private SelectMenu _selectMenu;
        private SelectMenu _firstTimeTutorialMenu;

        private Texture2D _titleTexture;
        private Texture2D _imageTexture;

        private Texture2D _tutorialPropmtTexture;

        private Rectangle _imageRegion;
        private Vector2 _titleLocation;

        private SpriteFont _font;

        private SoundEffectInstance _backgroundSong;
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
            _selectMenu = new MainSelectMenu(menuRegion, centerOffset, menuItemSize, explanationRegion, this.SettingsHandler.SoundPlayer);
            _selectMenu.AddItem(new MainMenuItem("New Game", StartGame));
            _selectMenu.AddItem(new MainMenuItem("Tutorial", StartTutorial));
            _selectMenu.AddItem(new MainMenuItem("Controls", ControlsTutorial));
            _selectMenu.AddItem(new MainMenuItem("Settings", SettingsMenu));
            _selectMenu.AddItem(new MainMenuItem("Quit", ExitGame));

            menuRegion = new Rectangle(0, Height/4, Width, Height);
            _firstTimeTutorialMenu = new SelectMenu(menuRegion, menuItemSize, MainSelectMenu.NoAction, explanationRegion, this.SettingsHandler.SoundPlayer);
            _firstTimeTutorialMenu.AddItem(new MainMenuItem("Yes", AcceptTutorialPrompt));
            _firstTimeTutorialMenu.AddItem(new MainMenuItem("No", DeclineTutorialPrompt));

            base.Initialize();
        }

        public override void LoadContent()
        {
            _imageTexture = Content.Load<Texture2D>("main_menu/teaser");
            _titleTexture = Content.Load<Texture2D>("main_menu/game_title");
            _font = Content.Load<SpriteFont>("fonts/General");
            _selectMenu.LoadContent(Content);
            _firstTimeTutorialMenu.LoadContent(Content);
            _backgroundSong = Content.Load<SoundEffect>("songs/basic_supermarket").CreateInstance();
            _tutorialPropmtTexture = Content.Load<Texture2D>("tutorial/tutorial_prompt");
            base.LoadContent();
            _backgroundSong.IsLooped = true;
            SettingsHandler.SoundPlayer.playSong(_backgroundSong, 1f);
            
            _supermarketNoiseInstance = Content.Load<SoundEffect>("sounds/supermarket_atmosphere").CreateInstance();
            _supermarketNoiseInstance.IsLooped = true;
            SettingsHandler.SoundPlayer.playSong(_supermarketNoiseInstance, 1f);
        }

        public override void UnloadContent() {
            Content.UnloadAsset("main_menu/teaser");
            Content.UnloadAsset("main_menu/game_title");
            Content.UnloadAsset("fonts/General");
            Content.UnloadAsset("songs/basic_supermarket");
            Content.UnloadAsset("sounds/supermarket_atmosphere");
            Content.UnloadAsset("tutorial/tutorial_prompt");
            _selectMenu.UnloadContent(Content);
            _firstTimeTutorialMenu.UnloadContent(Content);
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (SettingsHandler.settings.firstLaunch) {
                _firstTimeTutorialMenu.Update(gameTime);
            } else {
                _selectMenu.Update(gameTime);
            }

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

            if (SettingsHandler.settings.firstLaunch) {
                Color pauseColor = new Color(122, 119, 110, 120);
                SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), pauseColor);

                Vector2 textureCenter = new Vector2(_tutorialPropmtTexture.Width / 2, _tutorialPropmtTexture.Height / 2);
                Vector2 textureLocation = new Vector2(Width / 2, Height / 3);
                SpriteBatch.Draw(_tutorialPropmtTexture, textureLocation, null, Color.White, 0f, textureCenter, 0.9f, SpriteEffects.None, 1f);
                _firstTimeTutorialMenu.Draw(SpriteBatch);
            } else {
                _selectMenu.Draw(SpriteBatch);
            }


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

        private void AcceptTutorialPrompt() {
            SettingsHandler.SetFirstLaunch(false);
            StartTutorial();
        }

        private void DeclineTutorialPrompt() {
            SettingsHandler.SetFirstLaunch(false);
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

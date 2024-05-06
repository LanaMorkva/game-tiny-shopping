using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.Game {

    public class TutorialScene : TinyShopping.Scene {

        private SpriteBatch _spriteBatch;

        private UIController _ui;

        private SoundController _sound;

        private SplitScreenHandler _splitScreenHandler;

        private Rectangle _player1Area;

        private Rectangle _player2Area;

        private SelectMenu _pauseMenu;

        public TutorialScene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
        }

        public override void Initialize() {
            _player1Area = new Rectangle(0, 0, Width / 2, Height);
            _player2Area = new Rectangle(Width / 2, 0, Width / 2, Height);
            _splitScreenHandler = new SplitScreenHandler(_player1Area, _player2Area, GraphicsDevice);
            _splitScreenHandler.Initialize();
            _ui = new UIController(GraphicsDevice, _splitScreenHandler, this);
            _sound = new SoundController(this);


            var menuRegion = new Rectangle(0, 0, Width, Height);
            var menuItemSize = new Vector2((int)(Width / 2.8), Height / 10);

            Rectangle explanationRegion = new Rectangle(50, Height - 150, 300, 100);
            List<MenuExplanation> explanations = new List<MenuExplanation> {
                new("<A>", "Select", Color.Green),
                new("<B>", "Resume Game", Color.Red)
            };
            _pauseMenu = new SelectMenu(menuRegion, menuItemSize, ResumeGame, explanationRegion, explanations);
            _pauseMenu.AddItem(new MenuItem("Resume", ResumeGame));
            _pauseMenu.AddItem(new MenuItem("Exit Game", LoadMainMenu));
            base.Initialize();
        }

        public override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _splitScreenHandler.LoadContent(Content);
            _ui.LoadContent(Content);
            _sound.LoadContent(Content);
            _pauseMenu.LoadContent(Content);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _splitScreenHandler.UnloadContent(Content);
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime) {
            if (!IsOver && IsStarted) {
                if (!IsPaused) {
                    _splitScreenHandler.Update(gameTime, this);
                    if (IsPaused) {
                        _pauseMenu.ResetActiveItem();
                    }
                } else {
                    _pauseMenu.Update(gameTime);
                }
            }

            _ui.Update(gameTime);
            _sound.Update(gameTime, _ui);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            _spriteBatch.Begin();
            
            Viewport original = GraphicsDevice.Viewport;
            _splitScreenHandler.Draw(_spriteBatch, gameTime);
            _ui.Draw(_spriteBatch, gameTime);
            GraphicsDevice.Viewport = original;

            if (IsPaused) {
                _spriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), new Color(122, 119, 110, 120), 0);
                _pauseMenu.Draw(_spriteBatch);
            }
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

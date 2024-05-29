using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.Game {

    public class GameScene : TinyShopping.Scene {
        private UIController _ui;

        private SoundController _sound;

        private World _world;

        private InsectHandler _insectHandler;

        private PheromoneHandler _pheromoneHandler;

        private SplitScreenHandler _splitScreenHandler;

        private SelectMenu _pauseMenu;
        private bool _newScene;

        public GameScene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
            _world = new World("map_isometric/map-angled");
            _pheromoneHandler = new PheromoneHandler(_world, SettingsHandler.SoundPlayer);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _world.FruitHandler, SettingsHandler.SoundPlayer);
            _splitScreenHandler = new SplitScreenHandler(this, _world, _insectHandler, _pheromoneHandler);
            _ui = new UIController(GraphicsDevice, _splitScreenHandler, this, _world);
            _sound = new SoundController(this);

            var menuRegion = new Rectangle(0, 0, Width, Height);
            var menuItemSize = new Vector2((int)(Width / 3.5), Height / 12);

            Rectangle explanationRegion = new Rectangle(50, Height - 150, 300, 100);
            List<MenuExplanation> explanations = new List<MenuExplanation> {
                new("<A>", "Select", Color.Green),
                new("<B>", "Resume Game", Color.Red)
            };
            _pauseMenu = new SelectMenu(menuRegion, menuItemSize, ResumeGame, explanationRegion, explanations, this.SettingsHandler.SoundPlayer);
            gameState = GameState.StartCountdown;
            _newScene = true;
        }

        public override void Initialize() {
            _splitScreenHandler.Initialize();
            _pauseMenu.AddItem(new MenuItem("Resume", ResumeGame));
            _pauseMenu.AddItem(new MenuItem("Settings", SettingsMenu));
            _pauseMenu.AddItem(new MenuItem("Controls", ControlsTutorial));
            _pauseMenu.AddItem(new MenuItem("Exit Game", LoadMainMenu));
            base.Initialize();
        }

        public override void LoadContent() {
            _world.LoadContent(Content, GraphicsDevice);
            if (_newScene) {
                _world.InitializeFruitHandler();
            }

            _insectHandler.LoadContent(Content, _newScene);
            _pheromoneHandler.LoadContent(Content);
            _splitScreenHandler.LoadContent(Content, _newScene);

            _ui.LoadContent(Content);
            _sound.LoadContent(Content);

            _pauseMenu.LoadContent(Content);
            base.LoadContent();

            _newScene = false;
        }

        public override void UnloadContent()
        {
            _world.UnloadContent(Content);
            _insectHandler.UnloadContent(Content);
            _pheromoneHandler.UnloadContent(Content);
            _splitScreenHandler.UnloadContent(Content);
            
            _ui.UnloadContent(Content);
            _sound.UnloadContent(Content);

            _pauseMenu.UnloadContent(Content);
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime) {
            if (gameState == GameState.Playing) {
                _insectHandler.Update(gameTime);
                _pheromoneHandler.Update(gameTime);
                _splitScreenHandler.Update(gameTime, this);
            } else if (gameState == GameState.Paused) {
                _pauseMenu.Update(gameTime);
            }

            _ui.Update(gameTime);
            _sound.Update(gameTime, _ui);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();
            
            Viewport original = GraphicsDevice.Viewport;
            _splitScreenHandler.Draw(SpriteBatch, gameTime);
            _ui.Draw(SpriteBatch, gameTime, _splitScreenHandler.GetPlayerPosition(PlayerIndex.One), _splitScreenHandler.GetPlayerPosition(PlayerIndex.Two), _splitScreenHandler.IsPlayerKeyboard(PlayerIndex.One), _splitScreenHandler. IsPlayerKeyboard(PlayerIndex.Two));
            GraphicsDevice.Viewport = original;

            if (gameState == GameState.Paused) {
                SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), new Color(122, 119, 110, 160), 0);
                _pauseMenu.Draw(SpriteBatch);
            }
            
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

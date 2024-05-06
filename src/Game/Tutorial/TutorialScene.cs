using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.Game {

    public class TutorialScene : TinyShopping.Scene {
        private UIController _ui;
        private SoundController _sound;
        private World _world;
        private InsectHandler _insectHandler;
        private PheromoneHandler _pheromoneHandler;
        private SplitScreenHandler _splitScreenHandler;
        private SelectMenu _pauseMenu;


        public TutorialScene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
            _world = new World("map_isometric/map-tutorial");
            _pheromoneHandler = new PheromoneHandler(_world);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _world.FruitHandler);
            _splitScreenHandler = new SplitScreenHandler(this, _world, _insectHandler, _pheromoneHandler);
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
        }

        public override void Initialize() {
            _splitScreenHandler.Initialize();
            _pauseMenu.AddItem(new MenuItem("Resume", ResumeGame));
            _pauseMenu.AddItem(new MenuItem("Exit Game", LoadMainMenu));
            base.Initialize();
        }

        public override void LoadContent() {
            _world.LoadContent(Content, GraphicsDevice);
            _insectHandler.LoadContent(Content);
            _pheromoneHandler.LoadContent(Content);
            _splitScreenHandler.LoadContent(Content);

            _ui.LoadContent(Content);
            _sound.LoadContent(Content);
            _pauseMenu.LoadContent(Content);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _world.UnloadContent(Content);
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
            SpriteBatch.Begin();
            
            Viewport original = GraphicsDevice.Viewport;
            _splitScreenHandler.Draw(SpriteBatch, gameTime);
            _ui.Draw(SpriteBatch, gameTime);
            GraphicsDevice.Viewport = original;

            if (IsPaused) {
                SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), new Color(122, 119, 110, 120), 0);
                _pauseMenu.Draw(SpriteBatch);
            }
            
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

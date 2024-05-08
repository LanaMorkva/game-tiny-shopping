using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.Game {

    public class TutorialScene : TinyShopping.Scene {

        enum TutorialPhase {
            None = -1,
            Begin = 0, 
            Phase1,
            Phase2,
            Phase3
        }

        private SoundController _sound;
        private World _world;
        private InsectHandler _insectHandler;
        private PheromoneHandler _pheromoneHandler;
        private SplitScreenHandler _splitScreenHandler;
        private SelectMenu _pauseMenu;
        private Texture2D _introTexture;
        private TutorialUIController _ui;
        private bool _tutorialPause = false;
        private SelectMenu _tutorialMenu;
        private TutorialPhase _tutorialPhase = TutorialPhase.None;
        private double _runtimeS;


        public TutorialScene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
            _world = new World("map_isometric/map-tutorial");
            _pheromoneHandler = new PheromoneHandler(_world);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _world.FruitHandler);
            _splitScreenHandler = new SplitScreenHandler(this, _world, _insectHandler, _pheromoneHandler);
            _ui = new TutorialUIController(GraphicsDevice, _splitScreenHandler, this);
            _sound = new SoundController(this);

            var menuItemSize = new Vector2((int)(Width / 2.8), Height / 10);
            Rectangle explanationRegion = new Rectangle(50, Height - 100, 300, 100);
            List<MenuExplanation> explanations = new List<MenuExplanation> {
                new("<A>", "Select", Color.Green),
                new("<B>", "Resume Game", Color.Red)
            };

            _pauseMenu = new SelectMenu(new Rectangle(0, 0, Width, Height), menuItemSize, ResumeGame, explanationRegion, explanations);
            _tutorialMenu = new SelectMenu(new Rectangle(0, Height / 4, Width, Height), menuItemSize, LoadMainMenu, explanationRegion, new List<MenuExplanation> {
                new("<A>", "Select", Color.Green)});
        }

        public override void Initialize() {
            _splitScreenHandler.Initialize();
            _pauseMenu.AddItem(new MenuItem("Resume", ResumeGame));
            _pauseMenu.AddItem(new MenuItem("Exit Game", LoadMainMenu));
            _tutorialMenu.AddItem(new MenuItem("Next", TutorialPhaseDone));
            _runtimeS = 0;
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
            _tutorialMenu.LoadContent(Content);

            _introTexture = Content.Load<Texture2D>("tutorial/intro");
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            //TODO: unload everything, so it wont hang in the ContentManager memory
            _world.UnloadContent(Content);
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime) {
            _runtimeS += gameTime.ElapsedGameTime.TotalSeconds;
            if (_runtimeS > 1 && _tutorialPhase == TutorialPhase.None) {
                _tutorialPause = true;
                _tutorialPhase = TutorialPhase.Begin;
            }


            if (!IsOver && IsStarted) {
                if (!IsPaused && !_tutorialPause) {
                    _insectHandler.Update(gameTime);
                    _pheromoneHandler.Update(gameTime);
                    if (IsPaused) {
                        _pauseMenu.ResetActiveItem();
                    }
                } else {
                    _pauseMenu.Update(gameTime);
                }
            }
            
            _splitScreenHandler.Update(gameTime, this);
            _tutorialMenu.Update(gameTime);
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

            Color pauseColor = new Color(122, 119, 110, 120);
            if (IsPaused) {
                SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), pauseColor, 0);
                _pauseMenu.Draw(SpriteBatch);
            }

            if (_tutorialPause) {
                SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), pauseColor, 0);
            }
            //TODO: fix this conundrum
            DrawCurrentTutorialPhase();
            if (_tutorialPause) {
                _tutorialMenu.Draw(SpriteBatch);
            }
            
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawCurrentTutorialPhase() {
            //TODO:
            // Core ideas: introduce mechanics gradually (first let players move camera, than window with explaining fruits, etc)
            // 1. Camera movement
            // 2. Ants (how do they move, why do they move, how they leave small trails)
            // 4. Pheromones (how to place(duration, range), types of pheromones, ants with food will react only to blue pheromone)
            // 3. Fruits (how to collect from boxes, where to drop off, how to exchange for ants)
            // 4. Goal (collect more fruits that opponent or kill opponent)
            // 5. Fighting?

            Vector2 screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);
            Vector2 textureCenter = new Vector2(_introTexture.Width / 2, _introTexture.Height / 2);

            switch (_tutorialPhase) {
                case TutorialPhase.Begin:
                    SpriteBatch.Draw(_introTexture, screenCenter, null, Color.White, 0f, textureCenter, 1.2f, SpriteEffects.None, 1f);
                    break;
                case TutorialPhase.Phase1: 
                    SpriteBatch.Draw(_introTexture, screenCenter, null, Color.White, 0f, textureCenter, 0.1f, SpriteEffects.None, 1f);
                    break;
                default:
                    return;
            }
        }

        private void TutorialPhaseDone() {
            //TODO: both players need to confirm that they read and understood
            _tutorialPause = false;
            _tutorialPhase += 1;
        }
    }
}

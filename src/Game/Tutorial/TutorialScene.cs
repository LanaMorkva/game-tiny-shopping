using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TinyShopping.Game.Tutorial;

namespace TinyShopping.Game {

    public class TutorialScene : TinyShopping.Scene {

        public enum TutorialPhase {
            None = -1,
            Intro, 
            MoveCamera,
            MoveCameraWaitingForPlayer,
            AntsIntro,
            AntsTrails,
            AntsInProgress,
            PheromoneInitialization,
            PheromoneIntro,
            CollectFood,
            ExchangeFood, 
            ExchangeFoodDone,
            Goal,
            FinalMessage,
            TutorialEnded
        }

        private SoundController _sound;
        private World _world;
        private InsectHandler _insectHandler;
        private PheromoneHandler _pheromoneHandler;
        private TutorialUIController _ui;
        private SplitScreenHandler _splitScreenHandler;
        private SelectMenu _pauseMenu;
        private TutorialMenu _tutorialMenu;
        private Texture2D _introTexture;
        private Texture2D _cameraTexture;
        private Texture2D _cameraWaitingTexture;
        private Texture2D _antsIntroTexture;
        private Texture2D _antsTrailsTexture;
        private Texture2D _antsInProgressTexture;
        private Texture2D _pheromonesTexture;
        private Texture2D _collectFoodTexture;
        private Texture2D _exchangeFoodTexture;
        private Texture2D _exchangeFoodDoneTexture;
        private Texture2D _goalTexture;
        private Texture2D _finalMessageTexture;
        private TutorialPhase _tutorialPhase = TutorialPhase.None;
        private double _runtimeS;
        private double _lastPhaseCompletedTimeS;

        private CameraTutorialData _cam1Movement;
        private CameraTutorialData _cam2Movement;
        private int _player1AntCount = 0;
        private int _player2AntCount = 0;


        public TutorialScene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
            _world = new World("map_isometric/map-tutorial");
            _pheromoneHandler = new PheromoneHandler(_world, SettingsHandler.SoundPlayer);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _world.FruitHandler, SettingsHandler.SoundPlayer);
            _splitScreenHandler = new SplitScreenHandler(this, _world, _insectHandler, _pheromoneHandler);
            _ui = new TutorialUIController(GraphicsDevice, _splitScreenHandler, this);
            _sound = new SoundController(this);

            var menuItemSize = new Vector2((int)(Width / 3), Height / 12);
            Rectangle explanationRegion = new Rectangle(50, Height - 100, 300, 100);
            _pauseMenu = new SelectMenu(new Rectangle(0, 0, Width, Height), menuItemSize, ResumeGame, explanationRegion, this.SettingsHandler.SoundPlayer);


            menuItemSize = new Vector2((int)(Width / 7), Height / 15);
            var tutorialMenuRect = new Rectangle((int)(Width / 2.5), (int)(Height / 2.5), Width, Height);
            
            _tutorialMenu = new TutorialMenu(tutorialMenuRect, Vector2.Zero, menuItemSize, LoadMainMenu, explanationRegion, this.SettingsHandler.SoundPlayer);
        }

        public override void Initialize() {
            _splitScreenHandler.Initialize();
            _pauseMenu.AddItem(new MenuItem("Resume", ResumeGame));
            _pauseMenu.AddItem(new MenuItem("Settings", SettingsMenu));
            _pauseMenu.AddItem(new MenuItem("Controls", ControlsTutorial));
            _pauseMenu.AddItem(new MenuItem("Quit Tutorial", LoadMainMenu));
            _tutorialMenu.AddItem(new MenuItem("Next", NextTutorialPhase));
            _runtimeS = 0;
            _lastPhaseCompletedTimeS = 0;
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

            _introTexture = Content.Load<Texture2D>("tutorial/game_intro");
            _cameraTexture = Content.Load<Texture2D>("tutorial/camera");
            _cameraWaitingTexture = Content.Load<Texture2D>("tutorial/camera_waiting");
            _antsIntroTexture = Content.Load<Texture2D>("tutorial/ants_intro");
            _antsInProgressTexture = Content.Load<Texture2D>("tutorial/ants_in_progress");
            _antsTrailsTexture = Content.Load<Texture2D>("tutorial/ants_trails");
            _pheromonesTexture = Content.Load<Texture2D>("tutorial/pheromones_intro");
            _collectFoodTexture = Content.Load<Texture2D>("tutorial/collect_food");
            _exchangeFoodTexture = Content.Load<Texture2D>("tutorial/exchange_food");
            _exchangeFoodDoneTexture = Content.Load<Texture2D>("tutorial/exchange_food_done");
            _goalTexture = Content.Load<Texture2D>("tutorial/goal");
            _finalMessageTexture = Content.Load<Texture2D>("tutorial/final_message");
            base.LoadContent();
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
            _tutorialMenu.UnloadContent(Content);
            Content.UnloadAsset("tutorial/game_intro");
            Content.UnloadAsset("tutorial/camera");
            Content.UnloadAsset("tutorial/camera_waiting");
            Content.UnloadAsset("tutorial/ants_intro");
            Content.UnloadAsset("tutorial/ants_in_progress");
            Content.UnloadAsset("tutorial/ants_trails");
            Content.UnloadAsset("tutorial/pheromones_intro");
            Content.UnloadAsset("tutorial/collect_food");
            Content.UnloadAsset("tutorial/exchange_food");
            Content.UnloadAsset("tutorial/exchange_food_done");
            Content.UnloadAsset("tutorial/goal");
            Content.UnloadAsset("tutorial/final_message");
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime) {
            _runtimeS += gameTime.ElapsedGameTime.TotalSeconds;
            if (_tutorialPhase == TutorialPhase.None && TutorialDelayDoneS(0.5f)) {
                NextTutorialPhase();
            }

            if (_tutorialPhase == TutorialPhase.MoveCamera) {
                _cam1Movement.Update(_splitScreenHandler.GetCameraPosition(0), _splitScreenHandler.GetZoomValue(0));
                _cam2Movement.Update(_splitScreenHandler.GetCameraPosition(1), _splitScreenHandler.GetZoomValue(1));
                if (_cam1Movement.Completed() || _cam2Movement.Completed()) {
                    NextTutorialPhase();
                }
            }

            if (_tutorialPhase == TutorialPhase.AntsIntro) {
                _splitScreenHandler.SetPlayerCursorTo(0, _world.GetSpawnPositions()[0]);
                _splitScreenHandler.SetPlayerCursorTo(1, _world.GetSpawnPositions()[1]);
                _splitScreenHandler.Camera1.LookAt(_world.GetSpawnPositions()[0]);
                _splitScreenHandler.Camera2.LookAt(_world.GetSpawnPositions()[1]);
            }

            if (_tutorialPhase == TutorialPhase.AntsInProgress && TutorialDelayDoneS(1.0f)) {
                //TODO: disable user pheromone spawning
                _insectHandler.Update(gameTime);
            }

            if (_tutorialPhase == TutorialPhase.PheromoneInitialization) {
                _insectHandler.ResetColonies(Content);
                _world.InitializeFruitHandler(true);
                 NextTutorialPhase();
            }

            if (_tutorialPhase == TutorialPhase.CollectFood && TutorialDelayDoneS(1.0f)) {
                _insectHandler.Update(gameTime);
                _pheromoneHandler.Update(gameTime);

                if (_splitScreenHandler.GetNumberOfFruits(0) > 4 || _splitScreenHandler.GetNumberOfFruits(1) > 4) {
                    NextTutorialPhase();
                    _player1AntCount = _splitScreenHandler.GetNumberOfAnts(0);
                    _player2AntCount = _splitScreenHandler.GetNumberOfAnts(1);
                }
            }

            if (_tutorialPhase == TutorialPhase.ExchangeFood) {
                _insectHandler.Update(gameTime);
                _pheromoneHandler.Update(gameTime);

                if (_splitScreenHandler.GetNumberOfAnts(0) > _player1AntCount ||_splitScreenHandler.GetNumberOfAnts(1) > _player2AntCount) {
                    NextTutorialPhase();
                }
            }

            if (_tutorialPhase == TutorialPhase.ExchangeFoodDone) {
                _insectHandler.Update(gameTime);
                _pheromoneHandler.Update(gameTime);
            }

            if (gameState == GameState.Playing) {
                if (_tutorialPhase == TutorialPhase.TutorialEnded) {
                    _insectHandler.Update(gameTime);
                    _pheromoneHandler.Update(gameTime);
                } else {
                    _tutorialMenu.Update(gameTime);
                }
            } else {
                _pauseMenu.Update(gameTime);
            }


            _splitScreenHandler.Update(gameTime, this);

            _ui.Update(gameTime);
            _sound.Update(gameTime, _ui);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp);
            
            Viewport original = GraphicsDevice.Viewport;
            _splitScreenHandler.Draw(SpriteBatch, gameTime);
            _ui.Draw(SpriteBatch, gameTime);
            GraphicsDevice.Viewport = original;

            DrawCurrentTutorialPhase();
            if (gameState == GameState.Paused) {
                PauseDrawBackground();
                _pauseMenu.Draw(SpriteBatch);
            }
#if DEBUG
            _ui.DrawString(SpriteBatch, "Current tutorial phase: " + _tutorialPhase.ToString(), new Vector2(Width / 4,  Height - 50), 0.3f);
#endif
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawCurrentTutorialPhase() {
            //TODO:
            // Core ideas: introduce mechanics gradually (first let players move camera, than window with explaining fruits, etc)
            // 1. Camera movement [Done]
            // 2. Ants (how do they move, why do they move, how they leave small trails) ANTS vs TERMITES
            // 4. Pheromones (how to place(duration, range), types of pheromones, ants with food will react only to blue pheromone)
            // 3. Fruits (player can exchange them for more ants or stash -> victory goal)
            // 4. Goal (collect more fruits that opponent- or kill opponent)
            // 5. Fighting?

            switch (_tutorialPhase) {
                case TutorialPhase.Intro:
                    DrawBigTutorialPanel(_introTexture, 1f);
                    break;
                case TutorialPhase.MoveCamera: 
                    DrawSmallTutorialPanel(_cameraTexture, 0.2f);
                    break;
                case TutorialPhase.MoveCameraWaitingForPlayer:
                    DrawSmallTutorialPanel(_cameraWaitingTexture, 0.2f);
                    _tutorialMenu.Draw(SpriteBatch);
                    break;
                case TutorialPhase.AntsIntro:
                    DrawBigTutorialPanel(_antsIntroTexture, 0.5f);
                    break;
                case TutorialPhase.AntsTrails:
                    DrawBigTutorialPanel(_antsTrailsTexture);
                    break;
                case TutorialPhase.AntsInProgress:
                    DrawSmallTutorialPanel(_antsInProgressTexture, 0.2f);
                    _tutorialMenu.Draw(SpriteBatch);
                    break;
                case TutorialPhase.PheromoneIntro:
                    DrawBigTutorialPanel(_pheromonesTexture, 1f);
                    break;
                case TutorialPhase.CollectFood:
                    //TODO: draw arrows to the food boxes and drop off
                    DrawSmallTutorialPanel(_collectFoodTexture, 0.2f);
                    break;
                case TutorialPhase.ExchangeFood:
                    DrawSmallTutorialPanel(_exchangeFoodTexture, 0.2f);
                    break;
                case TutorialPhase.ExchangeFoodDone:
                    DrawSmallTutorialPanel(_exchangeFoodDoneTexture, 1.0f);
                    _tutorialMenu.Draw(SpriteBatch);
                    break;
                case TutorialPhase.Goal:
                    DrawBigTutorialPanel(_goalTexture, 1.0f);
                    break;
                case TutorialPhase.FinalMessage:
                    DrawBigTutorialPanel(_finalMessageTexture, 1.0f);
                    break;
                default:
                    return;
            }
        }

        private void DrawBigTutorialPanel(Texture2D texture, float fadeIn = 0f) {
            float timeDelta = (float)(_runtimeS - _lastPhaseCompletedTimeS);
            float alpha = fadeIn == 0 ? 1f : Math.Clamp(timeDelta / fadeIn, 0, 1);

            Vector2 screenCenter = new Vector2(Width / 2, Height / 2);
            Vector2 textureCenter = new Vector2(texture.Width / 2, texture.Height / 2);

            PauseDrawBackground(alpha);
            SpriteBatch.Draw(texture, screenCenter, null, Color.White * alpha, 0f, textureCenter, 1.0f, SpriteEffects.None, 1f);
            _tutorialMenu.Draw(SpriteBatch);
        }

        private void DrawSmallTutorialPanel(Texture2D texture, float fadeIn = 0f) {
            float timeDelta = (float)(_runtimeS - _lastPhaseCompletedTimeS);
            float alpha = fadeIn == 0 ? 1f : Math.Clamp(timeDelta / fadeIn, 0, 1);

            Vector2 screenCenter = new Vector2(Width / 2, Height / 2);
            Vector2 textureCenter = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 textureLocation = new Vector2(screenCenter.X, texture.Height / 4 + 20);
            SpriteBatch.Draw(texture, textureLocation, null, Color.White * alpha, 0f, textureCenter, 0.6f, SpriteEffects.None, 1f);
        }

        private void PauseDrawBackground(float alpha = 1f) {
            Color pauseColor = new Color(122, 119, 110, 120);
            SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), pauseColor * alpha, 0);
        }

        private void NextTutorialPhase() {
            //TODO: both players need to confirm that they read and understood
            // update: or do they?
            gameState = GameState.Playing;
            _tutorialPhase += 1;
            _ui.SetTutorialPhase(_tutorialPhase, _runtimeS);
            _lastPhaseCompletedTimeS = _runtimeS;
        }

        private bool TutorialDelayDoneS(float delay) {
            return (_runtimeS - _lastPhaseCompletedTimeS) > delay;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TinyShopping.Game.Tutorial;

namespace TinyShopping.Game {

    public class TutorialScene : TinyShopping.Scene {

        enum TutorialPhase {
            None = -1,
            Intro, 
            MoveCamera,
            MoveCameraWaitingForNext,
            AntsPause,
            AntsIntro,
            AntsTrails,
            AntsInProgress,
        }

        private SoundController _sound;
        private World _world;
        private InsectHandler _insectHandler;
        private PheromoneHandler _pheromoneHandler;
        private TutorialUIController _ui;
        private SplitScreenHandler _splitScreenHandler;
        private SelectMenu _pauseMenu;
        private SelectMenu _tutorialMenu;
        private Texture2D _introTexture;
        private Texture2D _cameraTexture;
        private Texture2D _cameraWaitingTexture;
        private Texture2D _antsIntroTexture;
        private Texture2D _antsTrailsTexture;
        private Texture2D _antsInProgressTexture;
        private bool _tutorialPause = false;
        private TutorialPhase _tutorialPhase = TutorialPhase.None;
        private double _runtimeS;
        private double _lastRecordedRuntimeS;

        private CameraTutorialData _cam1Movement;
        private CameraTutorialData _cam2Movement;


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
            _tutorialMenu = new SelectMenu(new Rectangle(0, (int)(Height / 2.5), Width, Height), menuItemSize, LoadMainMenu, explanationRegion);
        }

        public override void Initialize() {
            _splitScreenHandler.Initialize();
            _pauseMenu.AddItem(new MenuItem("Resume", ResumeGame));
            _pauseMenu.AddItem(new MenuItem("Exit Game", LoadMainMenu));
            _tutorialMenu.AddItem(new MenuItem("Next", TutorialPhaseDone, Color.DimGray));
            _runtimeS = 0;
            _lastRecordedRuntimeS = 0;
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
            if (_runtimeS > 0.2 && _tutorialPhase == TutorialPhase.None) {
                _tutorialPause = true;
                _tutorialPhase = TutorialPhase.Intro;
            }

            if (_tutorialPhase == TutorialPhase.MoveCamera) {
                _cam1Movement.Update(_splitScreenHandler.GetCameraPosition(0), _splitScreenHandler.GetZoomValue(0));
                _cam2Movement.Update(_splitScreenHandler.GetCameraPosition(1), _splitScreenHandler.GetZoomValue(1));
                if (_cam1Movement.Completed() || _cam2Movement.Completed()) {
                    TutorialPhaseDone();
                    _lastRecordedRuntimeS = _runtimeS;
                }
            }

            if (_tutorialPhase == TutorialPhase.AntsPause && (_runtimeS - _lastRecordedRuntimeS) > 3.0) {
                _tutorialPause = true;
                _tutorialPhase = TutorialPhase.AntsIntro;

                _splitScreenHandler.SetPlayerCursorTo(0, _world.GetSpawnPositions()[0]);
                _splitScreenHandler.SetPlayerCursorTo(1, _world.GetSpawnPositions()[1]);
                _splitScreenHandler.Camera1.LookAt(_world.GetSpawnPositions()[0]);
                _splitScreenHandler.Camera2.LookAt(_world.GetSpawnPositions()[1]);
            }

            if (_tutorialPhase == TutorialPhase.AntsTrails) {
                _tutorialPause = true;
            }

            if (_tutorialPhase == TutorialPhase.AntsInProgress) {
                _insectHandler.Update(gameTime);
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
            SpriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp);
            
            Viewport original = GraphicsDevice.Viewport;
            _splitScreenHandler.Draw(SpriteBatch, gameTime);
            _ui.Draw(SpriteBatch, gameTime);
            GraphicsDevice.Viewport = original;

            if (IsPaused) {
                PauseDrawBackground();
                _pauseMenu.Draw(SpriteBatch);
            }

            DrawCurrentTutorialPhase();
            
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawCurrentTutorialPhase() {
            //TODO:
            // Core ideas: introduce mechanics gradually (first let players move camera, than window with explaining fruits, etc)
            // 1. Camera movement [Done]
            // 2. Ants (how do they move, why do they move, how they leave small trails) ANTS vs TERMITES
            // 4. Pheromones (how to place(duration, range), types of pheromones, ants with food will react only to blue pheromone)
            // 3. Fruits (how to collect from boxes, where to drop off, how to exchange for ants)
            // 4. Goal (collect more fruits that opponent- or kill opponent)
            // 5. Fighting?

            switch (_tutorialPhase) {
                case TutorialPhase.Intro:
                    DrawBigTutorialPanel(_introTexture);
                    break;
                case TutorialPhase.MoveCamera: 
                    DrawBigTutorialPanel(_cameraTexture);
                    break;
                case TutorialPhase.MoveCameraWaitingForNext:
                    DrawSmallTutorialPanel(_cameraWaitingTexture);
                    break;
                case TutorialPhase.AntsIntro:
                    DrawBigTutorialPanel(_antsIntroTexture);
                    break;
                case TutorialPhase.AntsTrails:
                    DrawBigTutorialPanel(_antsTrailsTexture);
                    break;
                case TutorialPhase.AntsInProgress:
                    DrawSmallTutorialPanel(_antsInProgressTexture);
                    break;
                default:
                    return;
            }
        }

        private void DrawBigTutorialPanel(Texture2D texture) {
            Vector2 screenCenter = new Vector2(Width / 2, Height / 2);
            Vector2 textureCenter = new Vector2(texture.Width / 2, texture.Height / 2);

            PauseDrawBackground();
            SpriteBatch.Draw(texture, screenCenter, null, Color.White, 0f, textureCenter, 1.0f, SpriteEffects.None, 1f);
            _tutorialMenu.Draw(SpriteBatch);
        }

        private void DrawSmallTutorialPanel(Texture2D texture) {
            Vector2 screenCenter = new Vector2(Width / 2, Height / 2);
            Vector2 textureCenter = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 textureLocation = new Vector2(screenCenter.X, texture.Height / 4 + 20);
            SpriteBatch.Draw(texture, textureLocation, null, Color.White, 0f, textureCenter, 0.6f, SpriteEffects.None, 1f);
            _tutorialMenu.Draw(SpriteBatch);
        }

        private void PauseDrawBackground() {
            Color pauseColor = new Color(122, 119, 110, 120);
            SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), pauseColor, 0);
        }

        private void TutorialPhaseDone() {
            //TODO: both players need to confirm that they read and understood
            // update: or do they?
            _tutorialPause = false;
            _tutorialPhase += 1;
        }
    }
}

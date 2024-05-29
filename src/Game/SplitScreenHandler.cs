using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;

namespace TinyShopping.Game {

    internal class SplitScreenHandler {

        public Rectangle Player1Area { get; private set; }

        public Rectangle Player2Area { get; private set; }

        private GraphicsDevice _device;

        private World _world;

        private InsectHandler _insectHandler;

        private PheromoneHandler _pheromoneHandler;

        private Player _player1;

        private Player _player2;
        public List<Control> Controls1 {get; private set;}
        public List<Control> Controls2 {get; private set;}

        private SpriteBatch _batch;

        public Camera2D Camera1 { get; private set; }
        public Camera2D Camera2 { get; private set; }

        private Viewport _viewport1;
        private Viewport _viewport2;

        private RenderTarget2D _renderTarget1;
        private RenderTarget2D _renderTarget2;

        /// <summary>
        /// Creates a new instance to handle rendering to two split screens.
        /// </summary>
        /// <param name="area1">The area where the first screen should be drawn.</param>
        /// <param name="area2">The area where the second screen should be drawn.</param>
        /// <param name="device">The device to render to.</param>
        public SplitScreenHandler(Scene scene, World world, InsectHandler insectHandler, PheromoneHandler pheromoneHandler) {

            Player1Area = new Rectangle(0, 0, scene.Width / 2, scene.Height);
            Player2Area = new Rectangle(scene.Width / 2, 0, scene.Width / 2, scene.Height);

            _world = world;
            _insectHandler = insectHandler;
            _pheromoneHandler = pheromoneHandler;

            _device = scene.GraphicsDevice;
            _batch = new SpriteBatch(_device);
            
            Camera1 = new Camera2D(Player1Area.Width, Player1Area.Height);
            Camera2 = new Camera2D(Player2Area.Width, Player2Area.Height);

            _viewport1 = new Viewport(Player1Area);
            _viewport2 = new Viewport(Player2Area);
            
            _renderTarget1 = new RenderTarget2D(_device, Player1Area.Width, Player1Area.Height);
            _renderTarget2 = new RenderTarget2D(_device, Player2Area.Width, Player2Area.Height);
        }

        /// <summary>
        /// Initializes the necessary data.
        /// </summary>
        public void Initialize() {
        }

        /// <summary>
        /// Loads the necessary content from disk.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content, bool newScene) {
            if (newScene) {
                var spawnPositions = _world.GetSpawnPositions();
                PlayerInput input1 = CreatePlayerInput(PlayerIndex.One, content);
                Controls1 = input1.Controls;
                _player1 = new Player(_pheromoneHandler, input1, _insectHandler, _world, 0, spawnPositions[0]);
                
                PlayerInput input2 = CreatePlayerInput(PlayerIndex.Two, content);
                Controls2 = input2.Controls;
                _player2 = new Player(_pheromoneHandler, input2, _insectHandler, _world, 1, spawnPositions[1]);
                
                var mapCenter = _world.GetWorldBoundary().Center.ToVector2();
                Camera1.LookAt(mapCenter);
                Camera2.LookAt(mapCenter);

            }

            _player1.LoadContent(content);
            _player2.LoadContent(content);
        }

        public void UnloadContent(ContentManager content) {
            _player1.UnloadContent(content);
            _player2.UnloadContent(content);
        }

        /// <summary>
        /// Updates the game objects.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>   
        public void Update(GameTime gameTime, Scene scene) {
            _player1.Update(gameTime, this, scene);
            _player2.Update(gameTime, this, scene);

            Camera1.Update();
            Camera2.Update();
        }

        /// <summary>
        /// Draws the game objects to the two screens.
        /// </summary>
        /// <param name="batch">The main sprite batch.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Matrix viewMatrix = Camera1.GetViewMatrix();
            _device.Viewport = _viewport1;
            _device.SetRenderTarget(_renderTarget1);
            _batch.Begin(transformMatrix: viewMatrix, blendState: BlendState.AlphaBlend);
            DrawAllGameObjects(_batch, 0, viewMatrix, gameTime);
            _batch.End();
            
            viewMatrix = Camera2.GetViewMatrix();
            _device.Viewport = _viewport2;
            _device.SetRenderTarget(_renderTarget2);
            _batch.Begin(transformMatrix: viewMatrix, blendState: BlendState.AlphaBlend);
            DrawAllGameObjects(_batch, 1, viewMatrix, gameTime);
            _batch.End();

            _device.SetRenderTarget(null);
            batch.Draw(_renderTarget1, Player1Area, Color.White);
            batch.Draw(_renderTarget2, Player2Area, Color.White);
        }

        /// <summary>
        /// Draws all game objects to the two player's sprite batches.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void DrawAllGameObjects(SpriteBatch batch, int playerId, Matrix viewMatrix, GameTime gameTime) {
            _world.DrawFloor(batch, viewMatrix, Vector2.Zero);
            _pheromoneHandler.Draw(batch, playerId, gameTime);
            _insectHandler.Draw(batch, gameTime, playerId);

            // need to flush sprites before rendering tiled map objects, to ensure that fruits, ants are drawn before map objects
            // this is needed, because tiled draws directly to the graphics device, while SpriteBatch draws only at the End() call
            batch.End();
            _batch.Begin(transformMatrix: viewMatrix);
            _world.DrawObjects(batch, viewMatrix, Vector2.Zero);
            _insectHandler.DrawForeground(batch);
            if (playerId == 0) {
                _player1.Draw(batch, gameTime);
            } else {
                _player2.Draw(batch, gameTime);
            }
            
#if DEBUG
            _world.DrawDebugInfo(batch);
#endif
        }

        /// <summary>
        /// Creates a fitting player input method for the given player.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <returns>A PlayerInput instance.</returns>
        private PlayerInput CreatePlayerInput(PlayerIndex playerIndex, ContentManager content) {
            GamePadState state = GamePad.GetState(playerIndex);
            if (state.IsConnected) {
                return new GamePadInput(playerIndex, content);
            }
            return new KeyboardInput(playerIndex, content);
        }

        /// <summary>
        /// Updates the given player's camera position.
        /// </summary>
        /// <param name="player">The current player.</param>
        /// <param name="cursorPos">The cursor world position.</param>
        /// <param name="speed">The speed of the scrolling.</param>
        public void UpdateCameraState(int player, Vector2 cursorPos, int speed, float zoom, Vector2 cameraMotion) {
            Camera2D currentCamera;
            if  (player == 0) {
                currentCamera = Camera1;
            } else {
                currentCamera = Camera2;
            }
            
            RectangleF cameraRect = currentCamera.BoundingRectangle();
            Vector2 cameraMoveDirection = cameraMotion * speed;
            Rectangle worldRect = _world.GetWorldBoundary();
            float moveThreshold = 100f / currentCamera.Zoom;
            if (cameraRect.Right - cursorPos.X < moveThreshold && cameraRect.Right < worldRect.Right) {
                cameraMoveDirection.X += speed; 
            }
            if (cursorPos.X - cameraRect.Left < moveThreshold && cameraRect.Left > worldRect.Left) {
                cameraMoveDirection.X -= speed; 
            }
            if (cursorPos.Y - cameraRect.Top < moveThreshold && cameraRect.Y > worldRect.Top) { 
                cameraMoveDirection.Y -= speed;
            }
            if (cameraRect.Bottom - cursorPos.Y < moveThreshold && cameraRect.Bottom < worldRect.Bottom) {
                cameraMoveDirection.Y += speed;
            }

            currentCamera.TargetMovement = cameraMoveDirection;
            if (zoom != 0f) {
                currentCamera.ZoomIn(zoom);
            }
        }

        public float GetZoomValue(int playerId) {
            return playerId == 0 ? Camera1.Zoom : Camera2.Zoom;
        }

        public Vector2 GetCameraPosition(int playerId) {
            return playerId == 0 ? Camera1.Position : Camera2.Position;
        }

        public void SetPlayerCursorTo(int playerId, Vector2 position) {
            if (playerId == 0) {
                _player1.SetCursorTo(position);
            } else {
                _player2.SetCursorTo(position);
            }
        }

        /// <summary>
        /// Gets the number of ants in the colony of the given player.
        /// </summary>
        /// <param name="player">The player to use.</param>
        /// <returns>The number of ants.</returns>
        public int GetNumberOfAnts(int player) {
            return _insectHandler.GetNumberOfAnts(player);
        }

        /// <summary>
        /// Gets the number of fruits in the colony of the given player.
        /// </summary>
        /// <param name="player">The player to use.</param>
        /// <returns>The number of fruits collected.</returns>
        public int GetNumberOfFruits(int player) {
            return _insectHandler.GetNumberOfFruits(player);
        }

        /// <summary>
        /// Gets all insects in the colony of the given player.
        /// </summary>
        /// <param name="player">The player to use.</param>
        /// <returns>A list of insects.</returns>
        public IList<Insect> GetAllInsects(int player) {
            return _insectHandler.GetAllInsects(player);
        }

        /// <summary>
        /// Returns the healt of the given player's spawn.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>The spawn health.</returns>
        public int GetSpawnHealth(int player) {
            return _insectHandler.GetSpawnHealth(player);
        }

        public Vector2 GetPlayerPosition(PlayerIndex player) {
            // Apply camera transformation to render get position on screen
            if (player == PlayerIndex.One) {
                Matrix transformMatrix = Camera1.GetViewMatrix();
                return Vector2.Transform(_player1.GetPosition(), transformMatrix);
            } else {
                // Shady code, should be solved more general for a more general splitscreen
                Matrix transformMatrix = Camera2.GetViewMatrix();
                return Vector2.Transform(_player2.GetPosition(), transformMatrix) + new Vector2(Player1Area.Width, 0);
            }
        }

        public Player GetPlayer(PlayerIndex index) {
            if (index == PlayerIndex.One) {
                return _player1;
            } else {
                return _player2;
            }
        }

        public bool IsPlayerKeyboard(PlayerIndex index) {
            if (index == PlayerIndex.One) {
                return _player1.IsPlayerKeyboard();
            } else {
                return _player2.IsPlayerKeyboard();
            }
        }

    }
}

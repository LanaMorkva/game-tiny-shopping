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

        private Texture2D _borderTexture;

        private World _world;

        private InsectHandler _insectHandler;

        private PheromoneHandler _pheromoneHandler;

        private Player _player1;

        private Player _player2;
        public List<Control> Controls1 {get; private set;}
        public List<Control> Controls2 {get; private set;}
        private Scene _scene;

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
        public SplitScreenHandler(Rectangle area1, Rectangle area2, GraphicsDevice device, Scene scene) {
            Player1Area = area1;
            Player2Area = area2;
            _device = device;
            _batch = new SpriteBatch(device);
            _scene = scene;
        }

        /// <summary>
        /// Initializes the necessary data.
        /// </summary>
        public void Initialize() {
            _world = new World();
            _pheromoneHandler = new PheromoneHandler(_world);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _world.FruitHandler);
            
            Camera1 = new Camera2D(Player1Area.Width, Player1Area.Height);
            Camera2 = new Camera2D(Player2Area.Width, Player2Area.Height);

            _viewport1 = new Viewport(Player1Area);
            _viewport2 = new Viewport(Player2Area);
            
            _renderTarget1 = new RenderTarget2D(_device, Player1Area.Width, Player1Area.Height);
            _renderTarget2 = new RenderTarget2D(_device, Player2Area.Width, Player2Area.Height);
        }

        /// <summary>
        /// Loads the necessary content from disk.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content) {
            _world.LoadContent(content, _device);
            _insectHandler.LoadContent(content);
            _pheromoneHandler.LoadContent(content);
            
            var spawnPositions = _world.GetSpawnPositions();

            PlayerInput input1 = CreatePlayerInput(PlayerIndex.One, content);
            Controls1 = input1.Controls;
            _player1 = new Player(_pheromoneHandler, input1, _insectHandler, _world, 0, spawnPositions[0]);
            _player1.LoadContent(content);
            Camera1.LookAt(spawnPositions[0]);
            Camera1.ZoomIn(0.5f);

            PlayerInput input2 = CreatePlayerInput(PlayerIndex.Two, content);
            Controls2 = input2.Controls;
            _player2 = new Player(_pheromoneHandler, input2, _insectHandler, _world, 1, spawnPositions[1]);
            _player2.LoadContent(content);
            Camera2.LookAt(spawnPositions[1]);
            Camera2.ZoomIn(0.5f);

            CreateBorderTexture(new Color(252, 239, 197), 3);
        }

        public void UnloadContent(ContentManager content) {
            _world.UnloadContent(content, _device);
        }

        /// <summary>
        /// Updates the game objects.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime, Scene scene) {
            _insectHandler.Update(gameTime);
            _pheromoneHandler.Update(gameTime);

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
        /// Creates black rectangles to place around the player views.
        /// </summary>
        private void CreateBorderTexture(Color color, int width) {
            _borderTexture = new Texture2D(_device, Player1Area.Width, Player1Area.Height);
            Color[] data = new Color[Player1Area.Width * Player1Area.Height];
            FillTextureRect(data, 0, 0, Player1Area.Width, Player2Area.Height, new Color(0, 0, 0, 0));
            FillTextureRect(data, 0, 0, Player1Area.Width, width, color);
            FillTextureRect(data, 0, Player1Area.Height - width, Player1Area.Width, width, color);
            FillTextureRect(data, 0, 0, width, Player1Area.Height, color);
            FillTextureRect(data, Player1Area.Width - width, 0, width, Player1Area.Height, color);
            _borderTexture.SetData(data);
        }

        /// <summary>
        /// Fills the given rectangle of a data array with the given color.
        /// </summary>
        /// <param name="data">The array to fill.</param>
        /// <param name="x">The x coordinate of the rectangle.</param>
        /// <param name="y">The y coordinate of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">The color to use.</param>
        private void FillTextureRect(Color[] data, int x, int y, int width, int height, Color color) {
            for (int j = y; j < y + height; ++j) {
                for (int i = x; i < x + width; ++i) {
                    data[i + j * Player1Area.Width] = color;
                }
            }
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
            currentCamera.ZoomIn(zoom);
        }

        public float GetZoomValue(int playerId) {
            return playerId == 0 ? Camera1.Zoom : Camera2.Zoom;
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
    }
}

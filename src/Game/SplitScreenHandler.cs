using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TinyShopping.Game {

    internal class SplitScreenHandler {

        private Rectangle _player1Area;

        private Rectangle _player2Area;

        private Rectangle _player1Camera;

        private Rectangle _player2Camera;

        private GraphicsDevice _device;

        private Texture2D _borderTexture;

        private World _world;

        private InsectHandler _insectHandler;

        private PheromoneHandler _pheromoneHandler;

        private FruitHandler _fruitHandler;

        private Player _player1;

        private Player _player2;

        private SpriteBatch _batch1;

        private SpriteBatch _batch2;

        private RenderTarget2D _renderTarget1;

        private RenderTarget2D _renderTarget2;

        /// <summary>
        /// Creates a new instance to handle rendering to two split screens.
        /// </summary>
        /// <param name="area1">The area where the first screen should be drawn.</param>
        /// <param name="area2">The area where the second screen should be drawn.</param>
        /// <param name="device">The device to render to.</param>
        public SplitScreenHandler(Rectangle area1, Rectangle area2, GraphicsDevice device) {
            _player1Area = area1;
            _player2Area = area2;
            _device = device;
            _batch1 = new SpriteBatch(device);
            _batch2 = new SpriteBatch(device);
        }

        /// <summary>
        /// Initializes the necessary data.
        /// </summary>
        public void Initialize() {
            _world = new World();
            _pheromoneHandler = new PheromoneHandler(_world);
            _fruitHandler = new FruitHandler(_world);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _fruitHandler);
        }

        /// <summary>
        /// Loads the necessary content from disk.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content) {
            _world.LoadContent(content);
            _insectHandler.LoadContent(content);
            _fruitHandler.LoadContent(content);
            _pheromoneHandler.LoadContent(content);

            PlayerInput input1 = CreatePlayerInput(PlayerIndex.One);
            _player1 = new Player(_pheromoneHandler, input1, _insectHandler, 0, _world.GetTopLeftOfTile(5, 3));
            _player1.LoadContent(content);

            PlayerInput input2 = CreatePlayerInput(PlayerIndex.Two);
            _player2 = new Player(_pheromoneHandler, input2, _insectHandler, 1, _world.GetTopLeftOfTile(54, 35));
            _player2.LoadContent(content);

            _player1Camera = new Rectangle(0, 0, _player1Area.Width, _player1Area.Height);
            _player2Camera = new Rectangle(_world.Width - _player2Area.Width, _world.Height - _player2Area.Height, _player2Area.Width, _player2Area.Height);
            _renderTarget1 = new RenderTarget2D(_device, _player1Area.Width, _player1Area.Height);
            _renderTarget2 = new RenderTarget2D(_device, _player2Area.Width, _player2Area.Height);

            CreateBorderTexture(new Color(252, 239, 197), 3);
        }

        /// <summary>
        /// Updates the game objects.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            _insectHandler.Update(gameTime);
            _pheromoneHandler.Update(gameTime);
            _player1.Update(gameTime, this);
            _player2.Update(gameTime, this);
        }

        /// <summary>
        /// Draws the game objects to the two screens.
        /// </summary>
        /// <param name="batch">The main sprite batch.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            _batch1.Begin();
            _batch2.Begin();
            DrawAllGameObjects(gameTime);
            DrawBatchesToTextures();
            DrawTexturesToScreen(batch);
            batch.Draw(_borderTexture, _player1Area, Color.White);
            batch.Draw(_borderTexture, _player2Area, Color.White);
        }

        /// <summary>
        /// Draws the two predrawn textures to the main batch and thereby to the screen.
        /// </summary>
        /// <param name="batch">The main screen sprite batch.</param>
        private void DrawTexturesToScreen(SpriteBatch batch) {
            _device.SetRenderTarget(null);
            batch.Draw(_renderTarget1, _player1Area, Color.White);
            batch.Draw(_renderTarget2, _player2Area, Color.White);
        }

        /// <summary>
        /// Draws the two sprite batches to two textures.
        /// </summary>
        private void DrawBatchesToTextures() {
            _device.SetRenderTarget(_renderTarget1);
            _device.Clear(Color.Black);
            _batch1.End();
            _device.SetRenderTarget(_renderTarget2);
            _device.Clear(Color.Black);
            _batch2.End();
        }

        /// <summary>
        /// Draws all game objects to the two player's sprite batches.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void DrawAllGameObjects(GameTime gameTime) {
            _world.DrawFloor(_batch1, new Rectangle(0, 0, _player1Area.Width, _player1Area.Height), _player1Camera);
            _world.DrawFloor(_batch2, new Rectangle(0, 0, _player2Area.Width, _player2Area.Height), _player2Camera);
            _insectHandler.Draw(this, gameTime);
            _fruitHandler.Draw(this, gameTime);
            _pheromoneHandler.Draw(this, gameTime);
            _world.DrawObjects(_batch1, new Rectangle(0, 0, _player1Area.Width, _player1Area.Height), _player1Camera);
            _world.DrawObjects(_batch2, new Rectangle(0, 0, _player2Area.Width, _player2Area.Height), _player2Camera);
            _player1.Draw(this, gameTime);
            _player2.Draw(this, gameTime);
#if DEBUG
            _world.DrawDebugInfo(this);
#endif
        }

        /// <summary>
        /// Creates black rectangles to place around the player views.
        /// </summary>
        private void CreateBorderTexture(Color color, int width) {
            _borderTexture = new Texture2D(_device, _player1Area.Width, _player1Area.Height);
            Color[] data = new Color[_player1Area.Width * _player1Area.Height];
            FillTextureRect(data, 0, 0, _player1Area.Width, _player2Area.Height, new Color(0, 0, 0, 0));
            FillTextureRect(data, 0, 0, _player1Area.Width, width, color);
            FillTextureRect(data, 0, _player1Area.Height - width, _player1Area.Width, width, color);
            FillTextureRect(data, 0, 0, width, _player1Area.Height, color);
            FillTextureRect(data, _player1Area.Width - width, 0, width, _player1Area.Height, color);
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
                    data[i + j * _player1Area.Width] = color;
                }
            }
        }

        /// <summary>
        /// Creates a fitting player input method for the given player.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        /// <returns>A PlayerInput instance.</returns>
        private PlayerInput CreatePlayerInput(PlayerIndex playerIndex) {
            GamePadState state = GamePad.GetState(playerIndex);
            if (state.IsConnected) {
                return new GamePadInput(playerIndex);
            }
            return new KeyboardInput(playerIndex);

        }

        /// <summary>
        /// Renders an object to the corresponding slpit screen(s).
        /// </summary>
        /// <param name="texture">The texture to use.</param>
        /// <param name="destination">The destination location in world coordinates.</param>
        public void RenderObject(Texture2D texture, Rectangle destination) {
            if (destination.Intersects(_player1Camera)) {
                Rectangle screen = new Rectangle(destination.X - _player1Camera.X, destination.Y - _player1Camera.Y, destination.Width, destination.Height);
                _batch1.Draw(texture, screen, Color.White);
            }
            if (destination.Intersects(_player2Camera)) {
                Rectangle screen = new Rectangle(destination.X - _player2Camera.X, destination.Y - _player2Camera.Y, destination.Width, destination.Height);
                _batch2.Draw(texture, screen, Color.White);
            }
        }

        /// <summary>
        /// Renders an object to the corresponding slpit screen(s).
        /// </summary>
        /// <param name="texture">The texture to use.</param>
        /// <param name="destination">The destination location in world coordinates.</param>
        /// <param name="rotation">The rotation to apply to the texture.</param>
        public void RenderObject(Texture2D texture, Rectangle destination, float rotation) {
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            if (destination.Intersects(_player1Camera)) {
                Rectangle screen = new Rectangle(destination.X - _player1Camera.X, destination.Y - _player1Camera.Y, destination.Width, destination.Height);
                _batch1.Draw(texture, screen, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            }
            if (destination.Intersects(_player2Camera)) {
                Rectangle screen = new Rectangle(destination.X - _player2Camera.X, destination.Y - _player2Camera.Y, destination.Width, destination.Height);
                _batch2.Draw(texture, screen, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Renders an object to the screen of the given player.
        /// </summary>
        /// <param name="texture">The texture to use.</param>
        /// <param name="destination">The destination location in world coordinates.</param>
        /// <param name="player">The player that should see the object.</param>
        /// <param name="color">The color to use.</param>
        public void RenderObject(Texture2D texture, Rectangle destination, int player, Color color) {
            if (player == 0 && destination.Intersects(_player1Camera)) {
                Rectangle screen = new Rectangle(destination.X - _player1Camera.X, destination.Y - _player1Camera.Y, destination.Width, destination.Height);
                _batch1.Draw(texture, screen, color);
            }
            if (player == 1 && destination.Intersects(_player2Camera)) {
                Rectangle screen = new Rectangle(destination.X - _player2Camera.X, destination.Y - _player2Camera.Y, destination.Width, destination.Height);
                _batch2.Draw(texture, screen, color);
            }
        }

        /// <summary>
        /// Renders an object to the screen of the given player.
        /// </summary>
        /// <param name="texture">The texture to use.</param>
        /// <param name="destination">The destination location in world coordinates.</param>
        /// <param name="player">The player that should see the object.</param>
        public void RenderObject(Texture2D texture, Rectangle destination, int player) {
            RenderObject(texture, destination, player, Color.White);
        }

        /// <summary>
        /// Updates the given player's camera position.
        /// </summary>
        /// <param name="player">The current player.</param>
        /// <param name="cursorPos">The cursor world position.</param>
        /// <param name="speed">The speed of the scrolling.</param>
        public void UpdateCameraPosition(int player, Vector2 cursorPos, int speed) {
            Rectangle c = _player1Camera;
            if (player == 1) {
                c = _player2Camera;
            }
            if (c.X + c.Width - cursorPos.X < 50 && c.X + c.Width < _world.Width) {
                c.X += speed;
            }
            if (cursorPos.X - c.X < 50 && c.X > 0) {
                c.X -= speed;
            }
            if (c.Y + c.Height - cursorPos.Y < 50 && c.Y + c.Height < _world.Height) {
                c.Y += speed;
            }
            if (cursorPos.Y - c.Y < 50 && c.Y > 0) {
                c.Y -= speed;
            }
            if (player == 0) {
                _player1Camera = c;
            }
            else {
                _player2Camera = c;
            }
        }

        /// <summary>
        /// Returns the bounds of the camera of the given player.
        /// </summary>
        /// <param name="player">The player to get the camera bounds.</param>
        /// <returns>A rectangle in world coordinates.</returns>
        public Rectangle GetPlayerCameraBounds(int player) {
            return player == 0 ? _player1Camera : _player2Camera;
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
    }
}

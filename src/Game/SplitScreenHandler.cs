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

        public SplitScreenHandler(Rectangle area1, Rectangle area2, GraphicsDevice device) {
            _player1Area = area1;
            _player2Area = area2;
            _device = device;
            _batch1 = new SpriteBatch(device);
            _batch2 = new SpriteBatch(device);
        }

        public void Initialize() {
            _world = new World();
            _pheromoneHandler = new PheromoneHandler(_world);
            _fruitHandler = new FruitHandler(_world);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _fruitHandler);
        }

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
            CreateBorderTexture();
        }

        public void Update(GameTime gameTime) {
            _insectHandler.Update(gameTime);
            _pheromoneHandler.Update(gameTime);
            _player1.Update(gameTime, this);
            _player2.Update(gameTime, this);
        }

        public void Draw(SpriteBatch batch, GameTime gameTime) {
            // start batches
            _batch1.Begin();
            _batch2.Begin();

            // draw all objects
            _world.DrawFloor(_batch1, new Rectangle(0, 0, _player1Area.Width, _player1Area.Height), _player1Camera);
            _world.DrawFloor(_batch2, new Rectangle(0, 0, _player2Area.Width, _player2Area.Height), _player2Camera);
            _insectHandler.Draw(this, gameTime);
            _fruitHandler.Draw(this, gameTime);
            _pheromoneHandler.Draw(this, gameTime);
            _world.DrawObjects(_batch1, new Rectangle(0, 0, _player1Area.Width, _player1Area.Height), _player1Camera);
            _world.DrawObjects(_batch2, new Rectangle(0, 0, _player2Area.Width, _player2Area.Height), _player2Camera);
            _player1.Draw(this, gameTime);
            _player2.Draw(this, gameTime);

            // draw debug info
#if DEBUG
            _world.DrawDebugInfo(this);
#endif

            // draw player 1 area to texture
            _device.SetRenderTarget(_renderTarget1);
            _device.Clear(Color.Black);
            _batch1.End();

            // draw player 2 area to texture
            _device.SetRenderTarget(_renderTarget2);
            _device.Clear(Color.Black);
            _batch2.End();

            // draw player 1 + 2 textures to screen
            _device.SetRenderTarget(null);
            batch.Draw(_renderTarget1, _player1Area, Color.White);
            batch.Draw(_renderTarget2, _player2Area, Color.White);

            // draw border overlay
            batch.Draw(_borderTexture, _player1Area, Color.White);
            batch.Draw(_borderTexture, _player2Area, Color.White);
        }

        /// <summary>
        /// Creates black rectangles to place around the player views.
        /// </summary>
        private void CreateBorderTexture() {
            _borderTexture = new Texture2D(_device, _player1Area.Width, _player1Area.Height);
            Color[] data = new Color[_player1Area.Width * _player1Area.Height];
            for (int i = 0; i < data.Length; i++) {
                data[i] = new Color(0, 0, 0, 0);
            }
            for (int i = 0; i < _player1Area.Width; i++) {
                data[i] = Color.Black;
                data[i + _player1Area.Width] = Color.Black;
                data[i + (_player1Area.Width * (_player1Area.Height - 2))] = Color.Black;
                data[i + (_player1Area.Width * (_player1Area.Height - 1))] = Color.Black;
            }
            for (int i = 0; i < _player1Area.Height; i++) {
                data[0 + _player1Area.Width * i] = Color.Black;
                data[1 + _player1Area.Width * i] = Color.Black;
                data[_player1Area.Width - 2 + _player1Area.Width * i] = Color.Black;
                data[_player1Area.Width - 1 + _player1Area.Width * i] = Color.Black;
            }
            _borderTexture.SetData(data);
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
            if (c.X + c.Width - cursorPos.X < 50) {
                c.X += speed;
            }
            if (cursorPos.X - c.X < 50) {
                c.X -= speed;
            }
            if (c.Y + c.Height - cursorPos.Y < 50) {
                c.Y += speed;
            }
            if (cursorPos.Y - c.Y < 50) {
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

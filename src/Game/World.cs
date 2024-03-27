using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping.Game {

    internal class World {

        public static int NUM_OF_SQUARES_WIDTH = 57;

        public static int NUM_OF_SQUARES_HEIGHT = 40;

        public static int SCROLL_SPEED = 10;

        private GraphicsDevice _device;

        private Texture2D _floorTexture;
        private Texture2D _objectsTexture;

        private Rectangle _worldRegion;

        private Rectangle[] _obstacles;

        public Rectangle WorldRegion => _worldRegion;

#if DEBUG
        private Texture2D _obstacleTexture;
#endif

        private float _tileSize;

        public float TileSize {
            get {
                if (_tileSize == 0) {
                    throw new Exception("Tile size is not yet known, call LoadContent first!");
                }
                return _tileSize;
            }
        }

        private Rectangle _player1Area;

        private Rectangle _player2Area;

        private Rectangle _player1Camera;

        private Rectangle _player2Camera;

        private Texture2D _borderTexture;

        public World(Rectangle area1, Rectangle area2, GraphicsDevice device) {
            _player1Area = area1;
            _player2Area = area2;
            _device = device;
        }

        /// <summary>
        /// Loads necessary data from disk.
        /// </summary>
        /// <param name="contentManager">The content manager of the main game.</param>
        public void LoadContent(ContentManager contentManager) {
            _floorTexture = contentManager.Load<Texture2D>("static_map_floor_small");
            _tileSize = _floorTexture.Width / (float)NUM_OF_SQUARES_WIDTH;
            _player1Camera = new Rectangle(0, 0, _player1Area.Width, _player1Area.Height);
            _player2Camera = new Rectangle(_floorTexture.Width - _player2Area.Width, _floorTexture.Height-_player2Area.Height, _player2Area.Width, _player2Area.Height);
            _objectsTexture = contentManager.Load<Texture2D>("static_map_else_small");
            CreateBorderTexture();
            CreateCollisionAreas();
#if DEBUG
            _obstacleTexture = contentManager.Load<Texture2D>("obstacle");
#endif
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
        /// Draws the world to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void DrawFloor(SpriteBatch batch, GameTime gameTime) {
            batch.Draw(_floorTexture, _player1Area, _player1Camera, Color.White);
            batch.Draw(_floorTexture, _player2Area, _player2Camera, Color.White);
        }

        /// <summary>
        /// Draws the shelves.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void DrawObjects(SpriteBatch batch, GameTime gameTime) {
            batch.Draw(_objectsTexture, _player1Area, _player1Camera, Color.White);
            batch.Draw(_objectsTexture, _player2Area, _player2Camera, Color.White);
#if DEBUG
            DrawObstacleDebugInfo(batch);
#endif
            batch.Draw(_borderTexture, _player1Area, Color.White);
            batch.Draw(_borderTexture, _player2Area, Color.White);
        }

#if DEBUG
        /// <summary>
        /// Draws red rectangles around obstacles.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        private void DrawObstacleDebugInfo(SpriteBatch batch) {
            foreach (var o in _obstacles) {
                float x = o.X * TileSize;
                float y = o.Y * TileSize;
                float w = o.Width * TileSize;
                float h = o.Height * TileSize;
                Rectangle r = new Rectangle((int)x, (int)y, (int)w, (int)h);
                if (_player1Camera.Intersects(r)) {
                    Rectangle intersection = Rectangle.Intersect(r, _player1Camera);
                    intersection.X -= _player1Camera.X - _player1Area.X;
                    intersection.Y -= _player1Camera.Y - _player1Area.Y;
                    batch.Draw(_obstacleTexture, intersection, Color.White);
                }
                if (_player2Camera.Intersects(r)) {
                    Rectangle intersection = Rectangle.Intersect(r, _player2Camera);
                    intersection.X -= _player2Camera.X - _player2Area.X;
                    intersection.Y -= _player2Camera.Y - _player2Area.Y;
                    batch.Draw(_obstacleTexture, intersection, Color.White);
                }
            }
        }
#endif

        /// <summary>
        /// Creates obstacles that are considered not walkable.
        /// </summary>
        private void CreateCollisionAreas() {
            _obstacles = new Rectangle[] { 
                new Rectangle(0, 0, 1, NUM_OF_SQUARES_HEIGHT), // left wall
                new Rectangle(0, 0, NUM_OF_SQUARES_WIDTH, 2), // top wall
                new Rectangle(NUM_OF_SQUARES_WIDTH-1, 0, 1, NUM_OF_SQUARES_HEIGHT), // right wall
                new Rectangle(0, NUM_OF_SQUARES_HEIGHT-1, NUM_OF_SQUARES_WIDTH, 1), // bottom wall
                new Rectangle(4, 0, 2, 3), // top counter
                new Rectangle(NUM_OF_SQUARES_WIDTH-3, NUM_OF_SQUARES_HEIGHT-6, 3, 2), // bottom counter
                new Rectangle(18, 0, 2, 11),
                new Rectangle(0, 15, 18, 3),
                new Rectangle(11, 18, 2, 8),
                new Rectangle(1, 28, 8, 3),
                new Rectangle(7, 31, 2, 3),
                new Rectangle(17, 24, 9, 3),
                new Rectangle(24, 22, 2, 2),
                new Rectangle(33, 25, 2, 14),
                new Rectangle(24, 30, 9, 3),
                new Rectangle(37, 18, 19, 3),
                new Rectangle(45, 10, 2, 8),
                new Rectangle(28, 11, 10, 3),
                new Rectangle(28, 14, 2, 4),
                new Rectangle(31, 2, 2, 4),
            };
        }

        /// <summary>
        /// Checks if the given position +- range is walkable.
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <param name="range">The range to include in the check.</param>
        /// <returns>True if walkable, false otherwise.</returns>
        public bool IsWalkable(int x, int y, int range) {
            foreach (Rectangle obstacle  in _obstacles) {
                float rightBorder = TileSize * obstacle.X + TileSize * obstacle.Width;
                float leftBorder = TileSize * obstacle.X;
                float topBorder = TileSize * obstacle.Y;
                float bottomBorder = TileSize * obstacle.Y + TileSize * obstacle.Height;
                if (x-range < rightBorder && x+range > leftBorder && y-range < bottomBorder && y+range > topBorder) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Aligns the given position to the center of the current grid tile.
        /// </summary>
        /// <param name="position">The position to align.</param>
        public Vector2 AlignPositionToGridCenter(Vector2 position) {
            int xRaw = (int) MathF.Floor((position.X) / TileSize);
            int yRaw = (int) MathF.Floor((position.Y) / TileSize);
            float x = xRaw * TileSize + TileSize / 2;
            float y = yRaw * TileSize + TileSize / 2;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Calculates the center of the tile at given position.
        /// </summary>
        /// <param name="tileX">X coordinate of the tile.</param>
        /// <param name="tileY">Y coordinate of the tile.</param>
        /// <returns>The center of the tile in screen pixel coordinates.</returns>
        public Vector2 GetCenterOfTile(int tileX, int tileY) {
            return new Vector2(TileSize * tileX + TileSize / 2, TileSize * tileY + TileSize / 2);
        }

        /// <summary>
        /// Calculates the top left of the given tile.
        /// </summary>
        /// <param name="tileX">X coordinate of the tile.</param>
        /// <param name="tileY">Y coordinate of the tile.</param>
        /// <returns>The top left position of the given tile.</returns>
        public Vector2 GetTopLeftOfTile(int tileX, int tileY) {
            return new Vector2(TileSize * tileX, TileSize * tileY);
        }

        /// <summary>
        /// Converts the given world position to a screen position.
        /// </summary>
        /// <param name="player">The player for which the coordinates should be calculated.</param>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <returns>A position in screen coordinates.</returns>
        public Vector2 ConvertToScreenPosition(int player, Vector2 worldPosition) {
            if (player == 0) {
                return worldPosition - new Vector2(_player1Camera.X, _player1Camera.Y) + new Vector2(_player1Area.X, _player1Area.Y);
            }
            else {
                return worldPosition - new Vector2(_player2Camera.X, _player2Camera.Y) + new Vector2(_player2Area.X, _player2Area.Y);
            }
        }

        /// <summary>
        /// Updates the given player's camera position.
        /// </summary>
        /// <param name="player">The current player.</param>
        /// <param name="cursorPos">The cursor world position.</param>
        public void UpdateCameraPosition(int player, Vector2 cursorPos) {
            Rectangle c = _player1Camera;
            if (player == 1) {
                c = _player2Camera;
            }
            if (c.X + c.Width - cursorPos.X < 50) {
                c.X += SCROLL_SPEED;
            }
            if (cursorPos.X -  c.X < 50) {
                c.X -= SCROLL_SPEED;
            }
            if (c.Y + c.Height - cursorPos.Y < 50) {
                c.Y += SCROLL_SPEED;
            }
            if (cursorPos.Y - c.Y < 50) {
                c.Y -= SCROLL_SPEED;
            }
            if (player == 0) {
                _player1Camera = c;
            }
            else {
                _player2Camera = c;
            }
        }

        /// <summary>
        /// Renders an insect to the split screen.
        /// </summary>
        /// <param name="batch">The sprite batch to use.</param>
        /// <param name="texture">The texture to use.</param>
        /// <param name="origin">The texture origin to use for rotation.</param>
        /// <param name="position">The texture world position.</param>
        /// <param name="size">The texture size.</param>
        /// <param name="rotation">The rotation to use.</param>
        public void RenderInsect(SpriteBatch batch, Texture2D texture, Vector2 origin, Vector2 position, int size, float rotation) {
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, size, size);
            if (bounds.Intersects(_player1Camera)) {
                Vector2 pos = ConvertToScreenPosition(0, position);
                Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
                batch.Draw(texture, destination, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            }
            if (bounds.Intersects(_player2Camera)) {
                Vector2 pos = ConvertToScreenPosition(1, position);
                Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
                batch.Draw(texture, destination, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Renders an object to the split screen.
        /// </summary>
        /// <param name="batch">The sprite batch to use.</param>
        /// <param name="texture">The texture to use.</param>
        /// <param name="position">The world position.</param>
        /// <param name="size">The texture size.</param>
        public void RenderObect(SpriteBatch batch, Texture2D texture, Vector2 position, int size) {
            Rectangle location = new Rectangle((int)(position.X - size / 2), (int)(position.Y - size / 2), size, size);
            if (location.Intersects(_player1Camera)) {
                Vector2 pos = ConvertToScreenPosition(0, position);
                Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
                batch.Draw(texture, destination, Color.White);
            }
            if (location.Intersects(_player2Camera)) {
                Vector2 pos = ConvertToScreenPosition(1, position);
                Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
                batch.Draw(texture, destination, Color.White);
            }
        }

        /// <summary>
        /// Renders a pheromone to the owner's screen.
        /// </summary>
        /// <param name="player">The owner of the pheromone.</param>
        /// <param name="batch">The sprite batch to use.</param>
        /// <param name="texture">The texture to use.</param>
        /// <param name="bounds">The bounds to draw to.</param>
        /// <param name="color">The color to use for the pheromone.</param>
        public void RenderPheromone(int player, SpriteBatch batch, Texture2D texture, Rectangle bounds, Color color) {
            if (player == 0 && bounds.Intersects(_player1Camera)) {
                Vector2 pos = ConvertToScreenPosition(0, new Vector2(bounds.X, bounds.Y));
                bounds.X = (int)pos.X;
                bounds.Y = (int)pos.Y;
                batch.Draw(texture, bounds, color);
            }
            if (player == 1 && bounds.Intersects(_player2Camera)) {
                Vector2 pos = ConvertToScreenPosition(1, new Vector2(bounds.X, bounds.Y));
                bounds.X = (int)pos.X;
                bounds.Y = (int)pos.Y;
                batch.Draw(texture, bounds, color);
            }
        }
    }
}

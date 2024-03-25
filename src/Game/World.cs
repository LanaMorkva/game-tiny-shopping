using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    internal class World {

        public static int NUM_OF_SQUARES_WIDTH = 57;

        public static int NUM_OF_SQUARES_HEIGHT = 40;

        private GraphicsDeviceManager _device;

        private Texture2D _floorTexture;
        private Texture2D _objectsTexture;

        private Rectangle _worldPosition;

        private Vector2 _offset;

        private Vector2 _screen;

        private Rectangle[] _obstacles;

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

        public World(GraphicsDeviceManager device) {
            _device = device;
        }

        /// <summary>
        /// Loads necessary data from disk.
        /// </summary>
        /// <param name="contentManager">The content manager of the main game.</param>
        public void LoadContent(ContentManager contentManager) {
            _floorTexture = contentManager.Load<Texture2D>("static_map_floor");
            _objectsTexture = contentManager.Load<Texture2D>("static_map_else");
            CalculateWorldPosition();
            CreateCollisionAreas();
#if DEBUG
            _obstacleTexture = contentManager.Load<Texture2D>("obstacle");
#endif
        }

        /// <summary>
        /// Draws the world to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void DrawFloor(SpriteBatch batch, GameTime gameTime) {
            batch.Draw(_floorTexture, _worldPosition, Color.White);
        }

        public void DrawObjects(SpriteBatch batch, GameTime gameTime) {
             batch.Draw(_objectsTexture, _worldPosition, Color.White);
#if DEBUG
            foreach (var o in _obstacles) {
                float x = o.X * TileSize + _offset.X;
                float y = o.Y * TileSize + _offset.Y;
                float w = o.Width * TileSize;
                float h = o.Height * TileSize;
                batch.Draw(_obstacleTexture, new Rectangle((int)x, (int)y, (int)w, (int)h), Color.White);
            }
#endif

        }

        /// <summary>
        /// Calculates the position and size of the map such that it is fully on screen.
        /// </summary>
        private void CalculateWorldPosition() {
            _screen = new Vector2(_device.PreferredBackBufferWidth, _device.PreferredBackBufferHeight);
            float ratio;
            if (_floorTexture.Height / _screen.Y > _floorTexture.Width / _screen.X) {
                ratio = _screen.Y / _floorTexture.Height;
            }
            else {
                ratio = _screen.X / _floorTexture.Width;
            }
            int worldWidth = (int)(_floorTexture.Width * ratio);
            int worldHeight = (int)(_floorTexture.Height * ratio);
            int xOffset = (int)((_screen.X - worldWidth) / 2.0);
            int yOffset = (int)((_screen.Y - worldHeight) / 2.0);
            _offset = new Vector2(xOffset, yOffset);
            _worldPosition = new Rectangle(xOffset, yOffset, worldWidth, worldHeight);
            _tileSize = _worldPosition.Width / (float)NUM_OF_SQUARES_WIDTH;
        }

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
                float rightBorder = _offset.X + TileSize * obstacle.X + TileSize * obstacle.Width;
                float leftBorder = _offset.X + TileSize * obstacle.X;
                float topBorder = _offset.Y + TileSize * obstacle.Y;
                float bottomBorder = _offset.Y + TileSize * obstacle.Y + TileSize * obstacle.Height;
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
            int xRaw = (int) MathF.Floor((position.X - _offset.X) / TileSize);
            int yRaw = (int) MathF.Floor((position.Y - _offset.Y) / TileSize);
            float x = xRaw * TileSize + TileSize / 2;
            float y = yRaw * TileSize + TileSize / 2;
            return new Vector2(x + _offset.X, y + _offset.Y);
        }

        /// <summary>
        /// Calculates the center of the tile at given position.
        /// </summary>
        /// <param name="tileX">X coordinate of the tile.</param>
        /// <param name="tileY">Y coordinate of the tile.</param>
        /// <returns>The center of the tile in screen pixel coordinates.</returns>
        public Vector2 GetCenterOfTile(int tileX, int tileY) {
            return new Vector2(_offset.X + TileSize * tileX + TileSize / 2, _offset.Y + TileSize * tileY + TileSize / 2);
        }

        /// <summary>
        /// Calculates the top left of the given tile.
        /// </summary>
        /// <param name="tileX">X coordinate of the tile.</param>
        /// <param name="tileY">Y coordinate of the tile.</param>
        /// <returns>The top left position of the given tile.</returns>
        public Vector2 GetTopLeftOfTile(int tileX, int tileY) {
            return new Vector2(_offset.X + TileSize * tileX, _offset.Y + TileSize * tileY);
        }
    }
}

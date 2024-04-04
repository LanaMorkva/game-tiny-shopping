using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace TinyShopping.Game {

    internal class World {

        enum LayerName {
            Floor = 0,
            FloorShadow,
            Walls,
            InteriorBackground,
            InteriorForeground,
            Decorations,
        };

        public static int NUM_OF_SQUARES_WIDTH = 57;

        public static int NUM_OF_SQUARES_HEIGHT = 40;

        private Rectangle[] _obstacles;

        public TiledMap _tiledMap;
        public TiledMapRenderer _tiledMapRenderer;

        public int Width { get; private set; }

        public int Height { get; private set; }

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

        /// <summary>
        /// Loads necessary data from disk.
        /// </summary>
        /// <param name="contentManager">The content manager of the main game.</param>
        public void LoadContent(ContentManager contentManager, GraphicsDevice device) {
            _tiledMap = contentManager.Load<TiledMap>("map/map_48x48");
            _tiledMapRenderer = new TiledMapRenderer(device, _tiledMap);
            Width = _tiledMap.WidthInPixels;
            Height = _tiledMap.HeightInPixels;
            _tileSize =_tiledMap.TileWidth;

            CreateCollisionAreas();
#if DEBUG
            _obstacleTexture = contentManager.Load<Texture2D>("obstacle");
#endif
        }

        /// <summary>
        /// Draws an area of the world to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="destination">The destination to draw to.</param>
        /// <param name="source">The source rectangle on the texture to use.</param>
        public void DrawFloor(SpriteBatch batch, Matrix viewMatrix, Vector2 position) {
            _tiledMapRenderer.Draw((int)LayerName.Floor, viewMatrix);
            _tiledMapRenderer.Draw((int)LayerName.FloorShadow, viewMatrix);
        }

        /// <summary>
        /// Draws an area of the shelves to the sprite batch.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void DrawObjects(SpriteBatch batch, Matrix viewMatrix, Vector2 position) {
            _tiledMapRenderer.Draw((int)LayerName.Walls, viewMatrix);
            _tiledMapRenderer.Draw((int)LayerName.InteriorBackground, viewMatrix);
            _tiledMapRenderer.Draw((int)LayerName.InteriorBackground, viewMatrix);
            _tiledMapRenderer.Draw((int)LayerName.Decorations, viewMatrix);
        }

#if DEBUG
        /// <summary>
        /// Draws red rectangles around obstacles.
        /// </summary>
        /// <param name="handler">The split screen handler to use.</param>
        public void DrawDebugInfo(SpriteBatch batch) {
            // Note: temporaly not valid, since obstacles are hardcoded, will update later
            // foreach (var o in _obstacles) {
            //     float x = o.X * TileSize;
            //     float y = o.Y * TileSize;
            //     float w = o.Width * TileSize;
            //     float h = o.Height * TileSize;
            //     Rectangle r = new Rectangle((int)x, (int)y, (int)w, (int)h);
            //     batch.Draw(_obstacleTexture, r, Color.White);
            // }
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
            return worldPosition;
            //if (player == 0) {
            //    return worldPosition - new Vector2(_player1Camera.X, _player1Camera.Y) + new Vector2(_player1Area.X, _player1Area.Y);
            //}
            //else {
            //    return worldPosition - new Vector2(_player2Camera.X, _player2Camera.Y) + new Vector2(_player2Area.X, _player2Area.Y);
            //}
        }

        /// <summary>
        /// Updates the given player's camera position.
        /// </summary>
        /// <param name="player">The current player.</param>
        /// <param name="cursorPos">The cursor world position.</param>
        public void UpdateCameraPosition(int player, Vector2 cursorPos) {
            //Rectangle c = _player1Camera;
            //if (player == 1) {
            //    c = _player2Camera;
            //}
            //if (c.X + c.Width - cursorPos.X < 50) {
            //    c.X += SCROLL_SPEED;
            //}
            //if (cursorPos.X -  c.X < 50) {
            //    c.X -= SCROLL_SPEED;
            //}
            //if (c.Y + c.Height - cursorPos.Y < 50) {
            //    c.Y += SCROLL_SPEED;
            //}
            //if (cursorPos.Y - c.Y < 50) {
            //    c.Y -= SCROLL_SPEED;
            //}
            //if (player == 0) {
            //    _player1Camera = c;
            //}
            //else {
            //    _player2Camera = c;
            //}
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
            //Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, size, size);
            //if (bounds.Intersects(_player1Camera)) {
            //    Vector2 pos = ConvertToScreenPosition(0, position);
            //    Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
            //    batch.Draw(texture, destination, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            //}
            //if (bounds.Intersects(_player2Camera)) {
            //    Vector2 pos = ConvertToScreenPosition(1, position);
            //    Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
            //    batch.Draw(texture, destination, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            //}
        }

        /// <summary>
        /// Renders an object to the split screen.
        /// </summary>
        /// <param name="batch">The sprite batch to use.</param>
        /// <param name="texture">The texture to use.</param>
        /// <param name="position">The world position.</param>
        /// <param name="size">The texture size.</param>
        public void RenderObect(SpriteBatch batch, Texture2D texture, Vector2 position, int size) {
            //Rectangle location = new Rectangle((int)(position.X - size / 2), (int)(position.Y - size / 2), size, size);
            //if (location.Intersects(_player1Camera)) {
            //    Vector2 pos = ConvertToScreenPosition(0, position);
            //    Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
            //    batch.Draw(texture, destination, Color.White);
            //}
            //if (location.Intersects(_player2Camera)) {
            //    Vector2 pos = ConvertToScreenPosition(1, position);
            //    Rectangle destination = new((int)pos.X, (int)pos.Y, size, size);
            //    batch.Draw(texture, destination, Color.White);
            //}
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
            //if (player == 0 && bounds.Intersects(_player1Camera)) {
            //    Vector2 pos = ConvertToScreenPosition(0, new Vector2(bounds.X, bounds.Y));
            //    bounds.X = (int)pos.X;
            //    bounds.Y = (int)pos.Y;
            //    batch.Draw(texture, bounds, color);
            //}
            //if (player == 1 && bounds.Intersects(_player2Camera)) {
            //    Vector2 pos = ConvertToScreenPosition(1, new Vector2(bounds.X, bounds.Y));
            //    bounds.X = (int)pos.X;
            //    bounds.Y = (int)pos.Y;
            //    batch.Draw(texture, bounds, color);
            //}
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended;
using System.Collections.Generic;

namespace TinyShopping.Game {

    internal class World {

        enum LayerName {
            BackgroundGroup = 0,
            Floor,
            Objects,
            Walls,
            Objects2,
        };

        private ObstacleLayer _obstacleLayer;
        private TiledMapRenderer _tiledMapRenderer;
        private TiledMap _tiledMap;
        private TiledMapEffect _tintEffect;

        public int Width { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// Loads necessary data from disk.
        /// </summary>
        /// <param name="contentManager">The content manager of the main game.</param>
        public void LoadContent(ContentManager contentManager, GraphicsDevice device) {
            _tintEffect = new TiledMapEffect(contentManager.Load<Effect>("shaders/TintMapEffect"));
            _tiledMap = contentManager.Load<TiledMap>("map_isometric/map-angled");
            _tiledMap.GetLayer("Walls").Offset = new Vector2(0, -96);
            _tiledMap.GetLayer("Objects").Offset = new Vector2(0, -64);
            _tiledMapRenderer = new TiledMapRenderer(device, _tiledMap);
            _obstacleLayer = new ObstacleLayer(_tiledMap);
            Width = _tiledMap.Width;
            Height = _tiledMap.Height;
        }

        /// <summary>
        /// Draws an area of the world to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="destination">The destination to draw to.</param>
        /// <param name="source">The source rectangle on the texture to use.</param>
        public void DrawFloor(SpriteBatch batch, Matrix viewMatrix, Vector2 position) {
            _tiledMapRenderer.Draw((int)LayerName.BackgroundGroup, viewMatrix, effect: _tintEffect);
            _tiledMapRenderer.Draw((int)LayerName.Floor, viewMatrix);
            _tiledMapRenderer.Draw((int)LayerName.Objects, viewMatrix);
        }

        /// <summary>
        /// Draws an area of the shelves to the sprite batch.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void DrawObjects(SpriteBatch batch, Matrix viewMatrix, Vector2 position) {
            _tiledMapRenderer.Draw((int)LayerName.Walls, viewMatrix);
            _tiledMapRenderer.Draw((int)LayerName.Objects2, viewMatrix);
        }

#if DEBUG
        /// <summary>
        /// Draws red rectangles around obstacles.
        /// </summary>
        /// <param name="handler">The split screen handler to use.</param>
        public void DrawDebugInfo(SpriteBatch batch) {
            _obstacleLayer.Draw(batch);
            batch.DrawRectangle(GetWorldBoundary(), Color.RoyalBlue, 5);
        }
#endif

        /// <summary>
        /// Checks if the given position +- range is walkable.
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <param name="range">The range to include in the check.</param>
        /// <returns>True if walkable, false otherwise.</returns>
        public bool IsWalkable(int x, int y, int range) {
            return !_obstacleLayer.HasCollision(x, y, range);
        }

        /// <summary>
        /// Calculates the center of the tile at given position.
        /// </summary>
        /// <param name="tileX">X coordinate of the tile.</param>
        /// <param name="tileY">Y coordinate of the tile.</param>
        /// <returns>The center of the tile in screen pixel coordinates.</returns>
        public Vector2 GetCenterOfTile(int tileX, int tileY) {
            Vector2 tile = GetTopLeftOfTile(tileX, tileY);
            return new Vector2(tile.X + _tiledMap.TileWidth / 2, tile.Y + _tiledMap.TileHeight / 2);
        } 

        /// <summary>
        /// Calculates the top left of the given tile.
        /// </summary>
        /// <param name="tileX">X coordinate of the tile.</param>
        /// <param name="tileY">Y coordinate of the tile.</param>
        /// <returns>The top left position of the given tile.</returns>
        public Vector2 GetTopLeftOfTile(int tileX, int tileY) {
            TiledMapTile tile = _tiledMap.GetLayer<TiledMapTileLayer>("Floor").GetTile((ushort)tileX, (ushort)tileY);
            return ConvertTileToScreenPosition(new Vector2(tile.X, tile.Y));
        }

        public List<Vector2> GetSpawnPositions() {
            TiledMapObject[] spawns = _tiledMap.GetLayer<TiledMapObjectLayer>("spawns").Objects;
            List<Vector2> spawnPositions = new List<Vector2>();
            foreach (TiledMapObject obj in spawns) {
                spawnPositions.Add(ConvertPosToScreenPosition(obj.Position));
            }
            return spawnPositions;
        }

        public List<Vector2> GetDropOffPositions() {
            TiledMapObject[] spawns = _tiledMap.GetLayer<TiledMapObjectLayer>("dropoffs").Objects;
            List<Vector2> spawnPositions = new List<Vector2>();
            foreach (TiledMapObject obj in spawns) {
                spawnPositions.Add(ConvertPosToScreenPosition(obj.Position));
            }
            return spawnPositions;
        }

        /// <summary>
        /// Return world boundary in screen coordinates
        /// </summary>
        public Rectangle GetWorldBoundary() {
            // in Monogame world (0,0) corresponds to screen (0,0), in tiled screen (0,0) is top left corner of canvas,
            // so we need to offset our origin to match coordinates
            Vector2 leftTop = new Vector2(-_tiledMap.HeightInPixels, 0); //this offset works, dont ask why :)
            Vector2 rightBottom = ConvertTileToScreenPosition(new Vector2(89, 28)) + new Vector2(_tiledMap.HeightInPixels, 0);
            return new Rectangle(leftTop.ToPoint(), rightBottom.ToPoint());
        }

        /// <summary>
        /// Converts the given world position to a screen position.
        /// </summary>
        public Vector2 ConvertPosToScreenPosition(Vector2 worldPosition) {
            return Utilities.worldPosToScreen(worldPosition, _tiledMap.TileHeight, _tiledMap.TileWidth);
        }

        public Vector2 ConvertTileToScreenPosition(Vector2 tileIdx) {
            return Utilities.worldTileToScreen(tileIdx, _tiledMap.TileHeight, _tiledMap.TileWidth);
        }
    }
}

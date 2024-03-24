using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
//using System.Drawing;

namespace TinyShopping.Game {

    internal class Pheromone {
        private static readonly int RANGE = 8;
        public Vector2 Position { private set; get; }

        private World _world;

        private int _tileSize;
        private int _pheromoneSize;

        private int _textureWidth;
        private int _textureHeight;

        private Texture2D _texture;
        private Color _color;

        private int _numTextures = 5;

        public int Priority { private set; get; }
        public int PheromoneRange => _pheromoneSize / 2;

        /// <summary>
        /// Creates a new pheromone spot.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="priority">The starting priority. This will decrease for each passing milisecond.</param>
        public Pheromone(Vector2 position, Texture2D texture, PheromoneType type, World world, int priority) {
            Position = position;
            _world = world;
            _tileSize = (int)_world.TileSize;
            _textureWidth = texture.Width;
            _textureHeight = texture.Height;
            _texture = texture;
            Priority = priority;
            _pheromoneSize = RANGE * _tileSize * 2;

            switch (type) {
                case PheromoneType.RETURN:
                    _color = Color.Blue;
                    break;
                default:
                    _color = Color.Green;
                    break;
            }
        }

        /// <summary>
        /// Draws the pheromone spot.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Rectangle bounds = new Rectangle((int)Position.X - PheromoneRange, (int)Position.Y - PheromoneRange,
                _pheromoneSize, _pheromoneSize);

            float priority = (Priority + 500) / 1000;

            int alpha = (int)(priority * 20) + 100;
            Color updateColor = new Color(_color, alpha);
            batch.Draw(_texture, bounds, updateColor);
        }

        /// <summary>
        /// Updates the pheromone. Decreases the priority.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            Priority -= (int) Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping.Game {

    internal class Pheromone {

        private static readonly int RANGE = 8;

        public Vector2 Position { private set; get; }

        private World _world;

        private int _tileSize;
        private int _pheromoneSize;

        private Texture2D _texture;
        private Color _color;

        public int Priority { private set; get; }
        public int PheromoneRange => _pheromoneSize / 2;

        public PheromoneType Type { private set; get; }

        public int Owner { private set; get; }

        /// <summary>
        /// Creates a new pheromone spot.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="priority">The starting priority. This will decrease for each passing milisecond.</param>
        /// <param name="type">The pheromone type.</param>
        /// <param name="owner">The player placing the pheromone.</param>
        public Pheromone(Vector2 position, Texture2D texture, World world, int priority, PheromoneType type, int owner) {
            Position = position;
            _world = world;
            _tileSize = (int)_world.TileWidth;
            _texture = texture;
            Priority = priority;
            Type = type;
            Owner = owner;
            _pheromoneSize = RANGE * _tileSize;

            switch (type) {
                case PheromoneType.RETURN:
                    _color = Color.Blue;
                    break;
                case PheromoneType.FIGHT:
                    _color = Color.Red;
                    break;
                default:
                    _color = Color.Green;
                    break;
            }
        }

        /// <summary>
        /// Draws the pheromone spot.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Rectangle destination = new Rectangle((int)Position.X - PheromoneRange, (int)Position.Y - PheromoneRange, _pheromoneSize, _pheromoneSize);
            float priority = (Priority + 500) / 1000;
            int alpha = (int)(priority * 15) + 50;
            Color updateColor = new Color(_color, alpha);
            batch.Draw(_texture, destination, updateColor);
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

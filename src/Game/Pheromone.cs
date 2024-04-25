using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping.Game {

    internal class Pheromone {

        public Vector2 Position { private set; get; }

        private World _world;

        private int _tileSize;

        private Texture2D _texture;
        private Color _color;

        public int Priority { private set; get; }

        public int Range { private set; get; }

        public int Duration { private set; get; }

        public PheromoneType Type { private set; get; }

        public int Owner { private set; get; }

        /// <summary>
        /// Creates a new pheromone spot.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="duration">The duration of the pheromone. This will decrease for each passing milisecond.</param>
        /// <param name="range">The pheromone effect range.</param>
        /// <param name="type">The pheromone type.</param>
        /// <param name="owner">The player placing the pheromone.</param>
        public Pheromone(Vector2 position, Texture2D texture, World world, int priority, int duration, int range, PheromoneType type, int owner) {
            Position = position;
            _world = world;
            _tileSize = (int)_world.TileWidth;
            _texture = texture;
            Priority = priority;
            Type = type;
            Owner = owner;
            Range = range;
            Duration = duration;
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
            Rectangle destination = new Rectangle((int)Position.X - Range, (int)Position.Y - Range, Range * 2, Range * 2);
            float priority = (Duration + 500) / 2000;
            int alpha = (int)(priority * 15) + 50;
            Color updateColor = new Color(_color, alpha);
            batch.Draw(_texture, destination, updateColor);
        }

        /// <summary>
        /// Updates the pheromone. Decreases the priority.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            Duration -= (int) Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}

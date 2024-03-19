﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    internal class Pheromone {

        public Vector2 Position { private set; get; }

        private World _world;

        private int _textureSize;

        private Texture2D _texture;

        public int Priority { private set; get; }

        /// <summary>
        /// Creates a new pheromone spot.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="priority">The starting priority. This will decrease for each passing milisecond.</param>
        public Pheromone(Vector2 position, Texture2D texture, World world, int priority) {
            Position = position;
            _world = world;
            _textureSize = (int)_world.TileSize;
            _texture = texture;
            Priority = priority;
        }

        /// <summary>
        /// Draws the pheromone spot.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Rectangle bounds = new Rectangle((int)(Position.X - _textureSize / 2f), (int)(Position.Y - _textureSize / 2f), _textureSize, _textureSize);
            batch.Draw(_texture, bounds, Color.White);
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

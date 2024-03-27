﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping.Game {

    internal class Fruit {

        public Vector2 Position { get; private set; }

        private Texture2D _texture;

        private int _size;

        private World _world;

        public Fruit(Vector2 position, Texture2D texture, int size, World world) {
            Position = position;
            _texture = texture;
            _size = size;
            _world = world;
        }

        /// <summary>
        /// Draws the fruit to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            _world.RenderObect(batch, _texture, Position, _size);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    internal class Fruit {

        public Vector2 Position { get; private set; }

        private Texture2D _texture;

        private int _size;

        public Fruit(Vector2 position, Texture2D texture, int size) {
            Position = position;
            _texture = texture;
            _size = size;
        }

        /// <summary>
        /// Draws the fruit to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            batch.Draw(_texture, new Rectangle((int)(Position.X-_size/2), (int)(Position.Y-_size/2), _size, _size), Color.White);
        }
    }
}

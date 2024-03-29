using Microsoft.Xna.Framework;
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
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Rectangle destination = new Rectangle((int)Position.X, (int)Position.Y, _size, _size);
            batch.Draw(_texture, destination, Color.White);
        }
    }
}

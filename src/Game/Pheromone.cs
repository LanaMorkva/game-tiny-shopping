using Microsoft.Xna.Framework;
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

        private SpriteFont _font;

        public int Priority { private set; get; }

        public PheromoneType Type { private set; get; }

        /// <summary>
        /// Creates a new pheromone spot.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="font">The font to use.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="priority">The starting priority. This will decrease for each passing milisecond.</param>
        /// <param name="type">The pheromone type.</param>
        public Pheromone(Vector2 position, Texture2D texture, SpriteFont font, World world, int priority, PheromoneType type) {
            Position = position;
            _world = world;
            _textureSize = (int)_world.TileSize;
            _texture = texture;
            _font = font;
            Priority = priority;
            Type = type;
        }

        /// <summary>
        /// Draws the pheromone spot.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Rectangle bounds = new Rectangle((int)(Position.X - _textureSize / 2f), (int)(Position.Y - _textureSize / 2f), _textureSize, _textureSize);
            batch.Draw(_texture, bounds, Color.White);
            String message = ((Priority+500) / 1000).ToString();
            Vector2 textSize = _font.MeasureString(message);
            batch.DrawString(_font, message, Position, Color.Black, 0, textSize / 2, 0.5f, SpriteEffects.None, 0);
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

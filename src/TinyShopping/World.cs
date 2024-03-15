using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLab.TinyShopping {

    internal class World {

        private static int NUM_OF_SQUARES_WIDTH = 57;

        private static int NUM_OF_SQUARES_HEIGHT = 40;

        private GraphicsDeviceManager _device;

        private Texture2D _worldTexture;

        private Rectangle _worldPosition;

        public float TileSize {
            get;
            private set;
        }

        public World(GraphicsDeviceManager device) {
            _device = device;
        }

        /// <summary>
        /// Loads necessary data from disk.
        /// </summary>
        /// <param name="contentManager">The content manager of the main game.</param>
        public void LoadContent(ContentManager contentManager) {
            _worldTexture = contentManager.Load<Texture2D>("static_map");
            this.calculateWorldPosition();
            
        }

        /// <summary>
        /// Draws the world to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            batch.Draw(_worldTexture, _worldPosition, Color.White);
        }

        /// <summary>
        /// Calculates the position and size of the map such that it is fully on screen.
        /// </summary>
        private void calculateWorldPosition() {
            float ratio;
            if (_worldTexture.Height / _device.PreferredBackBufferHeight > _worldTexture.Width / _device.PreferredBackBufferWidth) {
                ratio = _device.PreferredBackBufferHeight / (float)_worldTexture.Height;
            }
            else {
                ratio = _device.PreferredBackBufferWidth / (float)_worldTexture.Width;
            }
            int worldWidth = (int)(_worldTexture.Width * ratio);
            int worldHeight = (int)(_worldTexture.Height * ratio);
            int xOffset = (int)((_device.PreferredBackBufferWidth - worldWidth) / 2.0);
            int yOffset = (int)((_device.PreferredBackBufferHeight - worldHeight) / 2.0);
            _worldPosition = new Rectangle(xOffset, yOffset, worldWidth, worldHeight);
            TileSize = _worldPosition.Width / (float)NUM_OF_SQUARES_WIDTH;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLab.TinyShopping {

    internal class PheromoneHandler {

        private World _world;

        private Texture2D _texture;

        private List<Pheromone> _pheromones;

        /// <summary>
        /// Creates a new pheromone handler.
        /// </summary>
        /// <param name="world">The world to exist in.</param>
        public PheromoneHandler(World world) {
            _world = world;
            _pheromones = new List<Pheromone>();
        }

        /// <summary>
        /// Loads the necessary files from disk.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager contentManager) {
            _texture = contentManager.Load<Texture2D>("circle");
        }

        /// <summary>
        /// Adds a new pheromone.
        /// </summary>
        /// <param name="rawPosition">The position of the player's cursor.</param>
        /// <param name="gameTime">The current game time.</param>
        public void AddPheromone(Vector2 rawPosition, GameTime gameTime) {
            Vector2 position = _world.AlignPositionToGridCenter(rawPosition);
            _pheromones.Add(new Pheromone(position, _texture, _world, (int) gameTime.TotalGameTime.TotalMilliseconds));
        }

        /// <summary>
        /// Updates the pheromones.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            int endIndex = _pheromones.Count;
            for (int i = 0; i < _pheromones.Count; ++i) {
                Pheromone p = _pheromones[i];
                if (gameTime.TotalGameTime.TotalMilliseconds - p.CreationTime < 5000) {
                    endIndex = i;
                    break;
                }
            }
            if (endIndex > 0) {
                _pheromones.RemoveRange(0, endIndex);
            }
        }

        /// <summary>
        /// Draws the pheromones.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            foreach (var p in _pheromones) {
                p.Draw(batch, gameTime);
            }
        }
    }
}

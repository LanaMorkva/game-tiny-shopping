using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    internal class PheromoneHandler {

        private static readonly int RANGE = 8;

        private World _world;

        private Texture2D _texture;

        private List<Pheromone> _pheromones;

        private List<Pheromone> _returnPheromones;

        /// <summary>
        /// Creates a new pheromone handler.
        /// </summary>
        /// <param name="world">The world to exist in.</param>
        public PheromoneHandler(World world) {
            _world = world;
            _pheromones = new List<Pheromone>();
            _returnPheromones = new List<Pheromone>();
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
        public void AddPheromone(Vector2 rawPosition, GameTime gameTime, PheromoneType type) {
            Vector2 position = _world.AlignPositionToGridCenter(rawPosition);
            Pheromone p = new Pheromone(position, _texture, _world, 5000);
            if (type == PheromoneType.RETURN) {
                _returnPheromones.Add(p);
            }
            else {
                _pheromones.Add(p);
            }
        }

        /// <summary>
        /// Updates the pheromones.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            _pheromones = UpdateAndRemovePheromones(_pheromones, gameTime);
            _returnPheromones = UpdateAndRemovePheromones(_returnPheromones, gameTime);
        }

        /// <summary>
        /// Updates the pheromones in the given list and filters out expired ones.
        /// </summary>
        /// <param name="pheromones">The pheromones to update.</param>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>A List with the pheromones still relevant.</returns>
        private List<Pheromone> UpdateAndRemovePheromones(List<Pheromone> pheromones, GameTime gameTime) {
            List<Pheromone> relevant = new List<Pheromone>(pheromones.Count);
            foreach (var p in pheromones) {
                p.Update(gameTime);
                if (p.Priority > 0) {
                    relevant.Add(p);
                }
            }
            return relevant;
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
            foreach (var p in _returnPheromones) {
                p.Draw(batch, gameTime);
            }
        }

        /// <summary>
        /// Gets the direction to the highest-priority forward pheromone in range.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <returns>A vector representing the direction or null if no pheromone is in range.</returns>
        public Vector2? GetDirectionToForwardPheromone(Vector2 position) {
            return GetDirectionToHighestPriorityPheromone(position, _pheromones);
        }

        /// <summary>
        /// Gets the direction to the highest-priority return pheromone in range.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <returns>A vector representing the direction or null if no pheromone is in range.</returns>
        public Vector2? GetDirectionToReturnPheromone(Vector2 position) {
            return GetDirectionToHighestPriorityPheromone(position, _returnPheromones);
        }

        /// <summary>
        /// Returns the pheromone with the highest priority in the list.
        /// </summary>
        /// <param name="position">The position of the insect to consider.</param>
        /// <param name="pheromones">The pheromones to search through.</param>
        /// <returns>Vector to the highest-priority pheromone in the list within range of the insect position.</returns>
        private Vector2? GetDirectionToHighestPriorityPheromone(Vector2 position, List<Pheromone> pheromones) {
            int range = (int)(RANGE * _world.TileSize);
            int maxPrio = 0;
            Pheromone closest = null;
            foreach (var p in pheromones) {
                if (p.Priority < maxPrio) {
                    continue;
                }
                float sqDis = Vector2.DistanceSquared(position, p.Position);
                if (sqDis < range * range) {
                    closest = p;
                    maxPrio = p.Priority;
                }
            }
            if (closest == null) {
                return null;
            }
            Vector2 direction = closest.Position - position;
            return direction;
        }
    }
}

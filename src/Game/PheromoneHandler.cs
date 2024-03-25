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

        private World _world;

        private Texture2D _texture;

        private SpriteFont _font;

        private List<Pheromone>[] _pheromones;

        private List<Pheromone>[] _returnPheromones;

        /// <summary>
        /// Creates a new pheromone handler for two players.
        /// </summary>
        /// <param name="world">The world to exist in.</param>
        public PheromoneHandler(World world) {
            _world = world;
            _pheromones = new List<Pheromone>[] { new (), new () };
            _returnPheromones = new List<Pheromone>[] { new(), new() };
        }

        /// <summary>
        /// Loads the necessary files from disk.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager contentManager) {
            _texture = contentManager.Load<Texture2D>("pheromone");
            _font = contentManager.Load<SpriteFont>("arial");
        }

        /// <summary>
        /// Adds a new pheromone.
        /// </summary>
        /// <param name="rawPosition">The position of the player's cursor.</param>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="type">The pheromone type to place.</param>
        /// <param name="player">The current player id, 0 or 1.</param>
        /// <param name="priority">The priority of the pheromone, given in miliseconds.</param>
        public void AddPheromone(Vector2 rawPosition, GameTime gameTime, PheromoneType type, int player, int priority) {
            Vector2 position = _world.AlignPositionToGridCenter(rawPosition);
            Pheromone p = new Pheromone(position, _texture, type, _world, priority);
            if (type == PheromoneType.RETURN) {
                _returnPheromones[player].Add(p);
            }
            else {
                _pheromones[player].Add(p);
            }
        }

        /// <summary>
        /// Updates the pheromones.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            UpdateAndRemovePheromones(_pheromones, gameTime);
            UpdateAndRemovePheromones(_returnPheromones, gameTime);
        }

        /// <summary>
        /// Updates the pheromones in the given list and filters out expired ones.
        /// </summary>
        /// <param name="pheromones">The pheromones to update.</param>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>A List with the pheromones still relevant.</returns>
        private void UpdateAndRemovePheromones(List<Pheromone>[] pheromones, GameTime gameTime) {
            for (int id = 0; id < pheromones.Length; ++id) {
                List<Pheromone> relevant = new List<Pheromone>(pheromones[id].Count);
                foreach (var p in pheromones[id]) {
                    p.Update(gameTime);
                    if (p.Priority > 0) {
                        relevant.Add(p);
                    }
                }
                pheromones[id] = relevant;
            }
        }

        /// <summary>
        /// Draws the pheromones.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            foreach (var ps in  _pheromones) {
                foreach (var p in ps) {
                    p.Draw(batch, gameTime);
                }
            }
            foreach (var ps in _returnPheromones) {
                foreach (var p in ps) {
                    p.Draw(batch, gameTime);
                }
            }
        }

        /// <summary>
        /// Gets the direction to the highest-priority forward pheromone in range.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <param name="player">The id of the current player, 0 or 1.</param>
        /// <returns>A vector representing the direction or null if no pheromone is in range.</returns>
        public Vector2? GetDirectionToForwardPheromone(Vector2 position, int player) {
            return GetDirectionToHighestPriorityPheromone(position, _pheromones[player]);
        }

        /// <summary>
        /// Gets the direction to the highest-priority return pheromone in range.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <param name="player">The id of the current palyer, 0 or 1.</param>
        /// <returns>A vector representing the direction or null if no pheromone is in range.</returns>
        public Vector2? GetDirectionToReturnPheromone(Vector2 position, int player) {
            return GetDirectionToHighestPriorityPheromone(position, _returnPheromones[player]);
        }

        /// <summary>
        /// Returns the pheromone with the highest priority in the list.
        /// </summary>
        /// <param name="position">The position of the insect to consider.</param>
        /// <param name="pheromones">The pheromones to search through.</param>
        /// <returns>Vector to the highest-priority pheromone in the list within range of the insect position.</returns>
        private Vector2? GetDirectionToHighestPriorityPheromone(Vector2 position, List<Pheromone> pheromones) {
            int maxPrio = 0;
            Pheromone closest = null;
            foreach (var p in pheromones) {
                if (p.Priority < maxPrio) {
                    continue;
                }
                float sqDis = Vector2.DistanceSquared(position, p.Position);
                int range = p.PheromoneRange;
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyShopping.Game {

    internal class PheromoneHandler {

        private World _world;

        private Texture2D _texture;

        private List<Pheromone>[] _pheromones;

        private List<Pheromone>[] _returnPheromones;

        private List<SoundEffect> _soundEffects;

        /// <summary>
        /// Creates a new pheromone handler for two players.
        /// </summary>
        /// <param name="world">The world to exist in.</param>
        public PheromoneHandler(World world) {
            _world = world;
            _pheromones = new List<Pheromone>[] { new (), new () };
            _returnPheromones = new List<Pheromone>[] { new(), new() };
            _soundEffects = new List<SoundEffect>();
        }

        /// <summary>
        /// Loads the necessary files from disk.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager contentManager) {
            _texture = contentManager.Load<Texture2D>("pheromone_particle");
            _soundEffects.Add(contentManager.Load<SoundEffect>("sounds/glass_knock"));
        }

        /// <summary>
        /// Adds a new pheromone.
        /// </summary>
        /// <param name="position">The position of the player's cursor.</param>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="type">The pheromone type to place.</param>
        /// <param name="player">The current player id, 0 or 1.</param>
        /// <param name="priority">The priority of the pheromone, given in miliseconds.</param>
        /// <param name="duration">The effect duration.</param>
        /// <param name="range">The effect range.</param>
        /// <param name="isPlayer">If the pheromone is placed by a player.</param>
        public void AddPheromone(Vector2 position, GameTime gameTime, PheromoneType type, int player, int priority, int duration, int range, bool isPlayer) {
            Pheromone p = new Pheromone(position, _texture, priority, duration, range, type, player, isPlayer);
            
            Pheromone closest = GetClosestPheromone(position, player, 100, type);
            if (isPlayer && closest != null) {
                closest.Dispose();
                if (type == PheromoneType.RETURN) {
                    _returnPheromones[player].Remove(closest);
                } else {
                    _pheromones[player].Remove(closest);
                }
            }


            if (type == PheromoneType.RETURN) {
                _returnPheromones[player].Add(p);
            }
            else {
                _pheromones[player].Add(p);
            }
            if (isPlayer) {
                _soundEffects[0].Play();
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
                    if (p.Duration > 0) {
                        relevant.Add(p);
                    } else {
                        p.Dispose();
                    }
                }
                pheromones[id] = relevant;
            }
        }

        /// <summary>
        /// Draws the pheromones.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch,int ownerId, GameTime gameTime) {
            foreach (var ps in  _pheromones) {
                foreach (var p in ps) {
                    if (ownerId == p.Owner) {
                        p.Draw(batch, gameTime);
                    }
                }
            }
            foreach (var ps in _returnPheromones) {
                foreach (var p in ps) {
                    if (ownerId == p.Owner) {
                        p.Draw(batch, gameTime);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the highest-priority forward pheromone in range.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <param name="player">The id of the current player, 0 or 1.</param>
        /// <returns>The pheromone instance or null if no pheromone is in range.</returns>
        public Pheromone GetForwardPheromone(Vector2 position, int player) {
            return GetHighestPriorityPheromone(position, _pheromones[player]);
        }

        /// <summary>
        /// Gets the highest-priority return pheromone in range.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <param name="player">The id of the current palyer, 0 or 1.</param>
        /// <returns>The pheromone instance or null if no pheromone is in range.</returns>
        public Pheromone GetReturnPheromone(Vector2 position, int player) {
            return GetHighestPriorityPheromone(position, _returnPheromones[player]);
        }

        /// <summary>
        /// Returns the pheromone with the highest priority in the list.
        /// </summary>
        /// <param name="position">The position of the insect to consider.</param>
        /// <param name="pheromones">The pheromones to search through.</param>
        /// <returns>Highest-priority pheromone in the list within range of the insect position.</returns>
        private Pheromone GetHighestPriorityPheromone(Vector2 position, List<Pheromone> pheromones) {
            int maxPrio = 0;
            Pheromone closest = null;
            foreach (var p in pheromones) {
                if (p.Priority < maxPrio) {
                    continue;
                }
                float sqDis = Vector2.DistanceSquared(position, p.Position);
                int range = p.Range;
                if (sqDis < range * range) {
                    closest = p;
                    maxPrio = p.Priority;
                }
            }
            return closest;
        }


        private Pheromone GetClosestPheromone(Vector2 position, int player, float range, PheromoneType type) {
            List<Pheromone> allPheromones = _pheromones[player].Concat(_returnPheromones[player]).ToList();
            Pheromone closest = null;
            range *= range;
            foreach (var p in allPheromones) {
                float sqDis = Vector2.DistanceSquared(position, p.Position);
                if (sqDis < range && p.Type == type) {
                    closest = p;
                    range = sqDis;
                }
            }
            return closest;
        }
    }
}

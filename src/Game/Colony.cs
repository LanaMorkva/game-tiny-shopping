using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    internal class Colony {

        private Vector2 _spawn;

        private int _spawnRotation;

        private World _world;

        private PheromoneHandler _handler;

        private FruitHandler _fruits;

        private Texture2D _antTexture;

        private Texture2D _antFullTexture;

        private List<Insect> _insects = new List<Insect>();

        private int _queue;

        private int _spawnCooldown;

        public Vector2 DropOff { get; private set; }

        private int _collectedFruit;

        private int _owner;

        private InsectHandler _insectHandler;

        /// <summary>
        /// Creates a new colony.
        /// </summary>
        /// <param name="spawn">The spawn point of the insects.</param>
        /// <param name="spawnRotation">The direction the ants need to walk to enter the map.</param>
        /// <param name="world">The world to live in.</param>
        /// <param name="handler">The pheromone handler to use.</param>
        /// <param name="fruits">The fruit handler to use.</param>
        /// <param name="dropOff">The position the ants can drop off fruit.</param>
        /// <param name="owner">The id of the owning player, 0 or 1.</param>
        /// <param name="insectHandler">The insect handler.</param>
        public Colony(Vector2 spawn, int spawnRotation, World world, PheromoneHandler handler, FruitHandler fruits, Vector2 dropOff, int owner, InsectHandler insectHandler) {
            _spawn = spawn;
            _world = world;
            _handler = handler;
            _fruits = fruits;
            _spawnRotation = spawnRotation;
            _queue = 6;
            DropOff = dropOff;
            _owner = owner;
            _insectHandler = insectHandler;
        }

        /// <summary>
        /// Initializes the colony and it's insects.
        /// </summary>
        public void Initialize() {
        }

        /// <summary>
        /// Loads the necessary data from disk.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _antTexture = content.Load<Texture2D>("ant_texture");
            _antFullTexture = content.Load<Texture2D>("ant_full_texture");
        }

        /// <summary>
        /// Updates the insects' position.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            _spawnCooldown -= (int) Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
            if (_spawnCooldown < 0 && _queue > 0) {
                _spawnCooldown = 1000;
                _queue -= 1;
                Insect ant = new Insect(_world, _handler, _spawn, _spawnRotation, _fruits, _antTexture, _antFullTexture, this, _owner);
                _insects.Add(ant);
            }
            List<Insect> remaining = new List<Insect>(_insects.Count);
            foreach (Insect insect in _insects) {
                if (insect.Health > 0) {
                    insect.Update(gameTime);
                    remaining.Add(insect);
                }
            }
            _insects = remaining;
        }

        /// <summary>
        /// Draws the insects.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
            foreach (Insect insect in _insects) {
                insect.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Reprots one more collected fruit.
        /// </summary>
        public void IncreaseFruitCount() {
            _collectedFruit += 1;
        }

        /// <summary>
        /// Trades a collected fruit for a new insect.
        /// </summary>
        public void BuyNewInsect() {
            if (_collectedFruit <= 0) {
                return;
            }
            _collectedFruit -= 1;
            _queue += 1;
        }

        /// <summary>
        /// Returns the closest enemy insect in range.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <returns>An insect instance or null.</returns>
        public Insect GetClosestEnemy(Vector2 position) {
            return _insectHandler.GetClosestEnemy(_owner, position);
        }

        /// <summary>
        /// Gets the closest insect to the given position.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <param name="range">The range to consider.</param>
        /// <returns>The closest insect.</returns>
        public Insect GetClosestToInRange(Vector2 position, float range) {
            float minDis = float.MaxValue;
            Insect closest = null;
            foreach (var i in _insects) {
                float sqDis = Vector2.DistanceSquared(position, i.Position);
                if (sqDis < range*range && sqDis < minDis) {
                    closest = i;
                    minDis = sqDis;
                }
            }
            return closest;
        }
    }
}

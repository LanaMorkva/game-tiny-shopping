using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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

        public int FruitsNum => _collectedFruit;

        public int AntsNum => _insects.Count;


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
        public Colony(Vector2 spawn, int spawnRotation, World world, PheromoneHandler handler, FruitHandler fruits, Vector2 dropOff, int owner) {
            _spawn = spawn;
            _world = world;
            _handler = handler;
            _fruits = fruits;
            _spawnRotation = spawnRotation;
            _queue = 6;
            DropOff = dropOff;
            _owner = owner;
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
            foreach (Insect insect in _insects) {
                insect.Update(gameTime);
            }
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
    }
}

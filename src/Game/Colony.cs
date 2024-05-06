using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.Linq;

namespace TinyShopping.Game {

    internal enum ColonyType {
        ANT = 0,
        TERMITE = 1,
    }

    internal class Colony {

        private Vector2 _spawn;

        private int _spawnRotation;

        private Services _services;

        private Texture2D _texture;

        public List<Insect> Insects { get; private set; } = new List<Insect>();

        private int _queue;

        private int _spawnCooldown;

        public Vector2 DropOff { get; private set; }

        private int _collectedFruit;

        private int _owner;

        private InsectHandler _insectHandler;

        private ColonyType _type;

        public int FruitsNum => _collectedFruit;

        public int AntsNum => Insects.Count;

        List<SoundEffect> _soundEffects;
        public List<Vector2> TargetPositions {get; set;}


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
        /// <param name="type">The insect type of this colony.</param>
        public Colony(Vector2 spawn, int spawnRotation, World world, PheromoneHandler handler, FruitHandler fruits, Vector2 dropOff, int owner, InsectHandler insectHandler, ColonyType type) {
            _spawn = spawn;
            _spawnRotation = spawnRotation;
            _queue = 6;
            DropOff = dropOff;
            _owner = owner;
            _insectHandler = insectHandler;
            _soundEffects = new List<SoundEffect>();
            _services = new Services { colony = this, fruits = fruits, handler = handler, world = world, coloniesHandler = insectHandler };
            _type = type;
            TargetPositions = new List<Vector2>();
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
            var texturePath = _type == ColonyType.ANT ? "ants/ant_texture" : "termites/termite_texture";
            _texture = content.Load<Texture2D>(texturePath);
            _soundEffects.Add(content.Load<SoundEffect>("sounds/cash_register"));
            _soundEffects.Add(content.Load<SoundEffect>("sounds/insect_dying"));
        }

        /// <summary>
        /// Updates the insects' position.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            TargetPositions.Clear();
            _spawnCooldown -= (int) Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
            if (_spawnCooldown < 0 && _queue > 0) {
                _spawnCooldown = 1000;
                _queue -= 1;
                Insect ant = GetNewInsect();
                Insects.Add(ant);
            }
            List<Insect> remaining = new List<Insect>(Insects.Count);
            foreach (Insect insect in Insects) {
                if (insect.Health > 0) {
                    insect.Update(gameTime);
                    remaining.Add(insect);
                } else {
                    _soundEffects[1].Play();
                }
            }
            Insects = remaining;
        }

        /// <summary>
        /// Creates a new insect of the correct type.
        /// </summary>
        /// <returns>An Ant or Termite.</returns>
        private Insect GetNewInsect() {
            return new Insect(_services, _spawn, _spawnRotation, _texture, _type);
        }

        /// <summary>
        /// Draws the insects.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime, bool playersColony) {
            foreach (Insect insect in Insects) {
                insect.Draw(batch, gameTime, playersColony);
            }
        }

        /// <summary>
        /// Reprots one more collected fruit.
        /// </summary>
        public void IncreaseFruitCount() {
            _collectedFruit += 1;
            _soundEffects[0].Play();
        }

        /// <summary>
        /// Trades a collected fruit for a new insect.
        /// </summary>
        public void BuyNewInsect() {
            if (_collectedFruit < Constants.INSECT_PRICE) {
                return;
            }
            _collectedFruit -= Constants.INSECT_PRICE;
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
            foreach (var i in Insects) {
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

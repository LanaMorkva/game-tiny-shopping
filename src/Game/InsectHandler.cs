using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;

namespace TinyShopping.Game {

    internal class Shot {
        public int Owner {get;}
        public int Damage {get;}
        public bool ShouldRemove {set; get;}
        private Vector2 _end;        
        private Vector2 _dir;        
        public Vector2 Position {get; private set;}

        public Shot(int owner, int damage, Vector2 start, Vector2 end) {
            Owner = owner;
            Damage = damage;
            Position = start;
            _end = end;
            _dir = (end - start).NormalizedCopy();
            ShouldRemove = false;
        }

        public void Update(GameTime gameTime) {
            Position += (float)gameTime.ElapsedGameTime.TotalSeconds * 160 * _dir;
            ShouldRemove = Vector2.Dot(_end - Position, _dir) < 0;
        }

        public void Draw(SpriteBatch batch) {
            // big thickness to draw filled circle
            if (Owner == 0) {
                batch.DrawCircle(Position, 3.5f, 8, new Color(66, 50, 38, 150), 3.5f);
                return;
            }
            batch.DrawCircle(Position, 4.5f, 8, new Color(130, 56, 7, 150), 4.5f);
        }
    }

    /// <summary>
    /// The InsectHandler takes care of the existing insect colonies.
    /// </summary>
    internal class InsectHandler {

        private World _world;

        private PheromoneHandler _pheromoneHandler;

        private FruitHandler _fruitHandler;

        private Colony[] _colonies;
        private List<Shot> _shots;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="world">The world to exist in.</param>
        /// <param name="pheromones">The pheromone handler to use.</param>
        /// <param name="fruits">The fruit handler to use.</param>
        public InsectHandler(World world, PheromoneHandler pheromones, FruitHandler fruits) {
            _world = world;
            _pheromoneHandler = pheromones;
            _fruitHandler = fruits;

            _shots = new List<Shot>();
        }

        /// <summary>
        /// Loads the colonies assets from disk.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content) {
            Colony colony1 = new Colony(_world.GetSpawnPositions()[0], 150, _world, _pheromoneHandler, _fruitHandler, _world.GetDropOffPositions()[0], 0, this, ColonyType.ANT);
            colony1.Initialize();
            colony1.LoadContent(content);
            Colony colony2 = new Colony(_world.GetSpawnPositions()[1], 230, _world, _pheromoneHandler, _fruitHandler, _world.GetDropOffPositions()[1], 1, this, ColonyType.TERMITE);
            colony2.Initialize();
            colony2.LoadContent(content);
            _colonies = new Colony[] { colony1, colony2 };
        }

        /// <summary>
        /// Updates the colonies' insects.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            foreach (var colony in _colonies) {
                colony.Update(gameTime);
            }
            foreach (var shot in _shots) {
                shot.Update(gameTime);
                int enemyIndex = 1 - shot.Owner;
                Colony c = _colonies[enemyIndex];
                float range = 10;
                var enemy = c.GetClosestToInRange(shot.Position, range);       
                if (enemy != null) {
                    enemy.TakeDamage(shot.Damage);
                    shot.ShouldRemove = true;
                }
            }
            _shots = _shots.Where(obj => !obj.ShouldRemove).ToList();
        }

        /// <summary>
        /// Draws the colonies' insects.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="playerId">The player's id whose screen we are drawing.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime, int playerId) {
            for (int i = 0; i < _colonies.Length; i++) {
                _colonies[i].Draw(batch, gameTime, playerId == i);
            }

            foreach (var shot in _shots) {
                shot.Draw(batch);
            }
        }

        /// <summary>
        /// Trades one fruit for a new insect for the given player's colony.
        /// </summary>
        /// <param name="player">The id of the player performing the action.</param>
        public void BuyNewInsect(int player) {
            _colonies[player].BuyNewInsect();
        }

        /// <summary>
        /// Returns the closest enemy insect.
        /// </summary>
        /// <param name="player">The current player requesting an enemy ant.</param>
        /// <param name="position">The position to compare to.</param>
        /// <returns>An instance of an enemy insect or null if none is in range.</returns>
        public Insect GetClosestEnemy(int player, Vector2 position) {
            int enemyIndex = 1 - player;
            Colony c = _colonies[enemyIndex];
            float range = Constants.ENEMY_VISIBILITY_RANGE;
            return c.GetClosestToInRange(position, range);            
        }

        /// <summary>
        /// Returns the drop off position for the given player.
        /// </summary>
        /// <param name="player">The player id.</param>
        /// <returns>The position of the drop off position.</returns>
        public Vector2 GetDropOff(int player) {
            return _colonies[player].DropOff;
        }

        /// <summary>
        /// Increases the given players fruit count.
        /// </summary>
        /// <param name="player">The player id.</param>
        public void IncreaseFruitCount(int player) {
            _colonies[player].IncreaseFruitCount();
        }

        /// <summary>
        /// Gets the number of ants in the colony of the given player.
        /// </summary>
        /// <param name="player">The player to use.</param>
        /// <returns>The number of ants.</returns>
        public int GetNumberOfAnts(int player) {
            return _colonies[player].AntsNum;
        }

        /// <summary>
        /// Gets the number of fruits in the colony of the given player.
        /// </summary>
        /// <param name="player">The player to use.</param>
        /// <returns>The number of fruits collected.</returns>
        public int GetNumberOfFruits(int player) {
            return _colonies[player].FruitsNum;
        }

        /// <summary>
        /// Gets all insects in the colony of the given player.
        /// </summary>
        /// <param name="player">The player to use.</param>
        /// <returns>A list of insects.</returns>
        public IList<Insect> GetAllInsects(int player) {
            return _colonies[player].Insects;
        }

        public void AddShot(int owner, int damagePower, Vector2 start, Vector2 end) {
            _shots.Add(new Shot(owner, damagePower, start, end));
        }

    }
}

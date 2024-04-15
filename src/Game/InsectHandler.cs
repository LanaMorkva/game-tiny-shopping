using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping.Game {

    /// <summary>
    /// The InsectHandler takes care of the existing insect colonies.
    /// </summary>
    internal class InsectHandler {

        private readonly static int ENEMY_VISIBILITY_RANGE = 2;

        private World _world;

        private PheromoneHandler _pheromoneHandler;

        private FruitHandler _fruitHandler;

        private Colony[] _colonies;

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
        }

        /// <summary>
        /// Loads the colonies assets from disk.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content) {
            Colony colony1 = new Colony(_world.GetSpawnPositions()[0], 150, _world, _pheromoneHandler, _fruitHandler, _world.GetDropOffPositions()[0], 0, this);
            colony1.Initialize();
            colony1.LoadContent(content);
            Colony colony2 = new Colony(_world.GetSpawnPositions()[1], 230, _world, _pheromoneHandler, _fruitHandler, _world.GetDropOffPositions()[1], 1, this);
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
        }

        /// <summary>
        /// Draws the colonies' insects.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            foreach(var colony in _colonies) {
                colony.Draw(batch, gameTime);
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
            float range = _world.TileWidth * ENEMY_VISIBILITY_RANGE;
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
    }
}

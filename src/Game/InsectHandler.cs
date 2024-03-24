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

    /// <summary>
    /// The InsectHandler takes care of the existing insect colonies.
    /// </summary>
    internal class InsectHandler {

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
            Colony colony1 = new Colony(_world.GetTopLeftOfTile(5, 0), 180, _world, _pheromoneHandler, _fruitHandler, _world.GetTopLeftOfTile(5, 3), 0);
            colony1.Initialize();
            colony1.LoadContent(content);
            Colony colony2 = new Colony(_world.GetTopLeftOfTile(57, 35), 270, _world, _pheromoneHandler, _fruitHandler, _world.GetTopLeftOfTile(54, 35), 1);
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
        /// <param name="batch">The sprite batch to draw to.</param>
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
    }
}

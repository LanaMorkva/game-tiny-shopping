using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game.AI {

    internal class PickUp : Task {

        private FruitHandler _fruits;

        /// <summary>
        /// Creates an AITask that handles picking up fruit.
        /// </summary>
        /// <param name="insect">The insect to control.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="fruits">The fruit handler to use.</param>
        public PickUp(Insect insect, World world, FruitHandler fruits) : base(insect, world) {
            _fruits = fruits;
        }

        /// <summary>
        /// Picks up close-by pieces of fruit.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if any action was taken, false otherwise.</returns>
        public override bool Run(GameTime gameTime) {
            if (Insect.IsCarrying) {
                return false;
            }
            Vector2? dir = _fruits.GetDirectionToClosestFruit(Insect.Position, out Fruit closestFruit);
            if (dir != null && dir.Value.LengthSquared() <= (World.TileSize * World.TileSize) / 4) {
                Insect.IsCarrying = true;
                _fruits.RemoveFruit(closestFruit);
                return true;
            }
            if (dir != null) {
                Insect.TargetDirection = dir.Value;
                Insect.Walk(gameTime);
                return true;
            }
            return false;
        }
    }
}

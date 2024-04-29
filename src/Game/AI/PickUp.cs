using Microsoft.Xna.Framework;

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
            if (Insect.IsCarrying || (Insect.Pheromone != null && Insect.Pheromone.Type == PheromoneType.FIGHT)) {
                return false; 
            }
            Rectangle insectBounds = new Rectangle(
                (int) Insect.Position.X, 
                (int) Insect.Position.Y, 
                Insect.TextureSize + Constants.PICKUP_RANGE, 
                Insect.TextureSize + Constants.PICKUP_RANGE
            );
            Vector2? dir = _fruits.GetDirectionToClosestFruit(Insect.Position, out Fruit closestFruit);
            if (dir != null && closestFruit.Contains(insectBounds)) {
                Insect.IsCarrying = true;
                closestFruit.EatFruit();
                return true;
            }
            if (dir != null) {
                Insect.WalkTo(closestFruit.BoundingBox.Center, Insect.Pheromone, gameTime);
                return true;
            }
            return false;
        }
    }
}

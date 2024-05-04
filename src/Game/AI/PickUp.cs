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
        public PickUp(Insect insect, World world, AIHandler aiHandler, FruitHandler fruits) : base(insect, world, aiHandler) {
            _fruits = fruits;
        }

        /// <summary>
        /// Picks up close-by pieces of fruit.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if any action was taken, false otherwise.</returns>
        public override bool Run(GameTime gameTime) {
            if (Insect.IsCarrying || (AIHandler.ActivePheromone != null && AIHandler.ActivePheromone.Type == PheromoneType.FIGHT)) {
                return false; 
            }
            int size = Insect.TextureSize + Constants.PICKUP_RANGE * 2;
            Rectangle insectBounds = new Rectangle(
                (int) Insect.Position.X - size / 2, 
                (int) Insect.Position.Y - size / 2, 
                size, 
                size
            );
            Vector2? dir = _fruits.GetDirectionToClosestFruit(Insect.Position, out Fruit closestFruit);
            if (dir != null && closestFruit.ShouldPickUp(insectBounds)) {
                Insect.IsCarrying = true;
                closestFruit.EatFruit();
                return true;
            }
            if (dir != null) {
                int maxOffset = closestFruit.Size / 3;
                AIHandler.WalkTo(closestFruit.Center, AIHandler.ActivePheromone, gameTime, InsectState.Run, maxOffset);
                return true;
            }
            return false;
        }
    }
}

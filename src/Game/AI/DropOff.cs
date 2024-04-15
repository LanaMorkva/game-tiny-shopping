using Microsoft.Xna.Framework;

namespace TinyShopping.Game.AI {

    internal class DropOff : Task {

        private Colony _colony;

        /// <summary>
        /// Creates an AITask that handles dopp-off of fruit.
        /// </summary>
        /// <param name="insect">The insect to control.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="colony">The insect's colony.</param>
        public DropOff(Insect insect, World world, Colony colony) : base(insect, world) {
            _colony = colony;
        }

        /// <summary>
        /// Drops off pieces of fruit at the colony's drop off counter..
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if any action was taken, false otherwise.</returns>
        public override bool Run(GameTime gameTime) {
            if (!Insect.IsCarrying || Vector2.DistanceSquared(Insect.Position, _colony.DropOff) > World.TileWidth * World.TileWidth) {
                return false;
            }
            Insect.IsCarrying = false;
            _colony.IncreaseFruitCount();
            return true;
        }
    }
}

using Microsoft.Xna.Framework;

namespace TinyShopping.Game.AI {

    internal class Spawn : Task {

        private bool _didSpawn = false;

        /// <summary>
        /// Creates an AITask that handles spawning of an Insect.
        /// </summary>
        /// <param name="insect">The insect to control.</param>
        /// <param name="world">The world to exist in.</param>
        public Spawn(Insect insect, World world, AIHandler aiHandler) : base(insect, world, aiHandler) {
        }

        /// <summary>
        /// Walks into the map until no more collisions are detected.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if any action was taken, false otherwise.</returns>
        public override bool Run(GameTime gameTime) {
            if (_didSpawn) {
                return false;
            }
            if (World.IsWalkable((int) Insect.Position.X, (int) Insect.Position.Y, Insect.TextureSize / 2)) {
                _didSpawn = true;
            }
            Insect.Walk(gameTime, Insect.InsectState.Run);
            return true;
        }
    }
}

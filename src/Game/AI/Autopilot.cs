using Microsoft.Xna.Framework;

namespace TinyShopping.Game.AI {

    internal class Autopilot : Task {

        private PheromoneHandler _handler;

        /// <summary>
        /// Creates an AITask that handles wandering of an Insect.
        /// </summary>
        /// <param name="insect">The insect to control.</param>
        /// <param name="world">The world to exist in.</param>
        public Autopilot(Insect insect, World world, AIHandler aiHandler, PheromoneHandler handler) : base(insect, world, aiHandler) {
            _handler = handler;
        }

        /// <summary>
        /// Walks into the map until no more collisions are detected.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if any action was taken, false otherwise.</returns>
        public override bool Run(GameTime gameTime) {
            bool isFightAlert = AIHandler.ActivePheromone?.Type == PheromoneType.FIGHT;
            if (isFightAlert) {
                AIHandler.Wander(gameTime, InsectState.FightWander);
                return true;
            }
            Pheromone p = Insect.IsCarrying ? _handler.GetReturnTrail(Insect.Position, Insect.Owner) :
                                              _handler.GetForwardTrail(Insect.Position, Insect.Owner);
            if (p != null) {
                AIHandler.WalkTo(p.Position, p, gameTime, Insect.IsCarrying ? InsectState.CarryRun : InsectState.Run);
            } else {
                AIHandler.Wander(gameTime, Insect.IsCarrying ? InsectState.CarryWander : InsectState.Wander);
            }
            return true;
        }
    }
}

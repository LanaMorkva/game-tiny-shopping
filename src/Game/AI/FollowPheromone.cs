using Microsoft.Xna.Framework;

namespace TinyShopping.Game.AI {

    internal class FollowPheromone : Task {

        private static readonly int FIGHT_RANGE = 1;

        private PheromoneHandler _handler;

        private Colony _colony;

        /// <summary>
        /// Creates an AITask that handles following pheromone spots.
        /// </summary>
        /// <param name="insect">The insect to control.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="handler">The pheromone handler to use.</param>
        /// <param name="colony">The insect's colony.</param>
        public FollowPheromone(Insect insect, World world, PheromoneHandler handler, Colony colony) : base(insect, world) {
            _handler = handler;
            _colony = colony;
        }

        /// <summary>
        /// Follows nearby pheromones and acts accordingly.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if any action was taken, false otherwise.</returns>
        public override bool Run(GameTime gameTime) {
            if (!Insect.IsCarrying) {
                return HandleForwardPheromones(gameTime);
            }
            else {
                return HandleReturnPheromone(gameTime);
            }
        }

        private bool HandleReturnPheromone(GameTime gameTime) {
            Pheromone p = _handler.GetReturnPheromone(Insect.Position, Insect.Owner);
            if (p == null) {
                return false;
            }
            Insect.TargetDirection = p.Position - Insect.Position;
            Insect.Walk(gameTime);
            return true;

        }

        private bool HandleForwardPheromones(GameTime gameTime) {
            Pheromone p = _handler.GetForwardPheromone(Insect.Position, Insect.Owner);
            if (p == null) {
                return false;
            }
            Vector2 direction = p.Position - Insect.Position;
            if (p.Type == PheromoneType.FIGHT) {
                Insect enemy = _colony.GetClosestEnemy(Insect.Position);
                if (enemy != null) {
                    direction = enemy.Position - Insect.Position;
                    float fightRange = World.TileWidth * FIGHT_RANGE;
                    if (Vector2.DistanceSquared(enemy.Position, Insect.Position) < fightRange * fightRange) {
                        enemy.TakeDamage(Insect.Damage);
                    }
                }
            }
            Insect.TargetDirection = direction;
            Insect.Walk(gameTime);
            return true;
        }
    }
}

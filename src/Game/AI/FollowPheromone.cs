using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace TinyShopping.Game.AI {

    internal class FollowPheromone : Task {

        private PheromoneHandler _handler;

        private Colony _colony;

        /// <summary>
        /// Creates an AITask that handles following pheromone spots.
        /// </summary>
        /// <param name="insect">The insect to control.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="handler">The pheromone handler to use.</param>
        /// <param name="colony">The insect's colony.</param>
        public FollowPheromone(Insect insect, World world, AIHandler aiHandler, PheromoneHandler handler, Colony colony) : base(insect, world, aiHandler) {
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
            if (p == null || p.ReachedInsects.Contains(Insect)) {
                return false;
            }
            AIHandler.WalkTo(p.Position, p, gameTime, InsectState.CarryRun);
            return true;

        }

        private bool HandleForwardPheromones(GameTime gameTime) {
            Pheromone p = _handler.GetForwardPheromone(Insect.Position, Insect.Owner);
            if (p == null) {
                return false;
            }
            if (p.Type == PheromoneType.FIGHT) {
                float fightRange = Constants.FIGHT_RANGE;
                Insect enemy = _colony.GetClosestEnemy(Insect.Position);
                if (enemy != null) {
                    if (Insect.CanGiveDamage && Vector2.DistanceSquared(enemy.Position, Insect.Position) < fightRange * fightRange) {
                        var shotEndPos = Insect.Position + (enemy.Position-Insect.Position).NormalizedCopy() * fightRange;
                        Insect.SendShot();
                        _colony.AddShot(Insect.GetDamagePower, Insect.Position, shotEndPos);
                    }                    
                    Vector2 target = enemy.Position;
                    Vector2 dirTarget = Vector2.Normalize(target - Insect.Position);
                    Vector2 offsetTarget = dirTarget * fightRange / 1.5f;
                    AIHandler.WalkTo(target - offsetTarget, p, gameTime, InsectState.Fight);
                    return true;
                }
                else if (Vector2.DistanceSquared(_colony.GetEnemySpawnPosition(), Insect.Position) < Constants.ENEMY_VISIBILITY_RANGE * Constants.ENEMY_VISIBILITY_RANGE) {
                    if (Insect.CanGiveDamage && Vector2.DistanceSquared(_colony.GetEnemySpawnPosition(), Insect.Position) < fightRange * fightRange * 2) {
                        var shotEndPos = Insect.Position + (_colony.GetEnemySpawnPosition() - Insect.Position).NormalizedCopy() * fightRange;
                        Insect.SendShot();
                        _colony.AddShot(Insect.GetDamagePower, Insect.Position, shotEndPos);
                    }
                    Vector2 target = _colony.GetEnemySpawnPosition();
                    Vector2 dirTarget = Vector2.Normalize(target - Insect.Position);
                    Vector2 offsetTarget = dirTarget * fightRange;
                    AIHandler.WalkTo(target - offsetTarget, p, gameTime, InsectState.Fight);
                    return true;
                }
            }
            if (p.ReachedInsects.Contains(Insect)) {
                return false;
            }
            AIHandler.WalkTo(p.Position, p, gameTime, p.Type == PheromoneType.FIGHT ? InsectState.FightRun : InsectState.Run);
            return true;
        }
    }
}

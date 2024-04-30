﻿using Microsoft.Xna.Framework;

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
            if (p == null || p == Insect.ReachedPheromone) {
                return false;
            }
            Insect.WalkTo(p.Position, p, gameTime);
            return true;

        }

        private bool HandleForwardPheromones(GameTime gameTime) {
            Pheromone p = _handler.GetForwardPheromone(Insect.Position, Insect.Owner);
            if (p == null) {
                return false;
            }
            if (p.Type == PheromoneType.FIGHT) {
                Insect enemy = _colony.GetClosestEnemy(Insect.Position);
                if (enemy != null) {
                    Vector2 target = enemy.Position;
                    float fightRange = Constants.FIGHT_RANGE;
                    if (Insect.CanGiveDamage && Vector2.DistanceSquared(enemy.Position, Insect.Position) < fightRange * fightRange) {
                        enemy.TakeDamage(Insect.GiveDamage);
                    }
                    Insect.WalkTo(target, p, gameTime);
                    return true;
                }
            }
            if (p == Insect.ReachedPheromone) {
                return false;
            }
            Insect.WalkTo(p.Position, p, gameTime);
            return true;
        }
    }
}

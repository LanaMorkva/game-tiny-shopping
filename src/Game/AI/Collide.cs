﻿using Microsoft.Xna.Framework;

namespace TinyShopping.Game.AI {

    internal class Collide : Task {

        private bool _isRecovering;

        /// <summary>
        /// Creates an AITask that handles collisions with the map.
        /// </summary>
        /// <param name="insect">The insect to control.</param>
        /// <param name="world">The world to exist in.</param>
        public Collide(Insect insect, World world, AIHandler aiHandler) : base(insect, world, aiHandler) {
        }

        /// <summary>
        /// Backs up and turns if the insect collides with any obstacle.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <returns>True if any action was taken, false otherwise.</returns>
        public override bool Run(GameTime gameTime) {
            if (_isRecovering) {
                RecoverCollision(gameTime);
                return true;
            }
            if (!World.IsWalkable((int) Insect.Position.X, (int) Insect.Position.Y, Insect.TextureSize / 5)) {
                _isRecovering = true;
                Insect.TargetRotation += 180;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Recovers from a collision with the map by backing up and turning.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void RecoverCollision(GameTime gameTime) {
            if (!Insect.IsTurning) {
                _isRecovering = false;
            }
            if (!World.IsWalkable((int)Insect.Position.X, (int)Insect.Position.Y, Insect.TextureSize / 5)) {
                Insect.BackUp(gameTime);
            }
            else {
                Insect.Rotate(gameTime);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TinyShopping.Game {

    internal class FruitHandler {

        private static readonly int RANGE = 2;

        private World _world;

        private Texture2D _appleTexture;

        private List<Fruit> _fruits;

        public FruitHandler(World world) { 
            _world = world;
            _fruits = new List<Fruit>();
        }

        /// <summary>
        /// Loads the necessary files from disk.
        /// </summary>
        /// <param name="contentManager">The content manager to use.</param>
        public void LoadContent(ContentManager content) {
            _appleTexture = content.Load<Texture2D>("apple");
            for (int i = 0; i < 50; i++) {
                int tileX = Random.Shared.Next(World.NUM_OF_SQUARES_WIDTH);
                int tileY = Random.Shared.Next(World.NUM_OF_SQUARES_WIDTH);
                Vector2 center = _world.GetCenterOfTile(tileX, tileY);
                if (_world.IsWalkable((int)center.X, (int)center.Y, 0)) {
                    _fruits.Add(new Fruit(center, _appleTexture, (int)(_world.TileSize*0.7), _world));
                }
            }
        }

        /// <summary>
        /// Draws the fruits.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            foreach (var fruit in _fruits) {
                fruit.Draw(batch, gameTime);
            }
        }

        /// <summary>
        /// Removes the given fruit.
        /// </summary>
        /// <param name="fruit">The fruit to remove.</param>
        public void RemoveFruit(Fruit fruit) {
            _fruits.Remove(fruit);
        }

        /// <summary>
        /// Gets the direction to the closest fruit in range.
        /// </summary>
        /// <param name="position">The position to compare to.</param>
        /// <param name="fruit">Will be set to the closest fruit instance.</param>
        /// <returns>A vector representing the direction or null if no fruit is in range.</returns>
        public Vector2? GetDirectionToClosestFruit(Vector2 position, out Fruit fruit) {
            int range = (int)(RANGE * _world.TileSize);
            float minDis = float.MaxValue;
            Fruit closest = null;
            foreach (var f in _fruits) {
                float sqDis = Vector2.DistanceSquared(position, f.Position);
                if (sqDis < minDis) {
                    closest = f;
                    minDis = sqDis;
                }
            }
            if (closest == null || minDis > range * range) {
                fruit = null;
                return null;
            }
            Vector2 direction = closest.Position - position;
            fruit = closest;
            return direction;
        }
    }
}

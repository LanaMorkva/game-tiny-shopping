using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TinyShopping.Game {

    internal class FruitHandler {

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
            GenerateFruits();
        }

        private void GenerateFruits() {
            float halfFruitSize = Constants.FRUIT_TEXTURE_SIZE / 2;
            for (int i = 0; i < Constants.FRUITS_NUM; ) {
                int tileX = Random.Shared.Next(_world.Width);
                int tileY = Random.Shared.Next(_world.Height);
                Vector2 center = _world.GetCenterOfTile(tileX, tileY);
                if (_world.IsWalkable((int)center.X, (int)center.Y, (int)halfFruitSize)) {
                    Vector2 position = new Vector2(center.X - halfFruitSize, center.Y - halfFruitSize);
                    int size = (int)(halfFruitSize * 2);
                    _fruits.Add(new Fruit(position, _appleTexture, size, _world));
                    i++;
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
            int range = Constants.FRUIT_VISIBILITY_RANGE;
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System.Linq;

namespace TinyShopping.Game {

    internal class FruitHandler {

        private World _world;

        private Texture2D _appleTexture;
        private Texture2D _boxAssets;
        private Texture2D _boxEmptyAssets;

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
            _boxAssets = content.Load<Texture2D>("map_isometric/asset-box");
            _boxEmptyAssets = content.Load<Texture2D>("map_isometric/asset-box-empty");
            GenerateFruitBoxes();
            GenerateFruits();
        }

        private void GenerateFruitBoxes() {
            foreach (var bottomLeftPos in _world.GetBoxPositions()) {
                _fruits.Add(new FruitBox(bottomLeftPos, _world, _boxAssets, _boxEmptyAssets));
            }
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
                    // add to the begining, so they would be drawn behind fruit baskets
                    _fruits.Insert(0, new FruitPiece(new RectangleF(position, new Size2(size, size)), _world, _appleTexture));
                    i++;
                }
            }
        }

        /// <summary>
        /// Checks for collisions with static obstacles
        /// </summary>
        /// <param name="objRect">Polygon that determines objects boundaries</param>
        public bool HasCollision(Polygon objRect) {
            return _fruits.Any(o => o.Collides(objRect));;
        }

        /// <summary>
        /// Draws the fruits.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        public void Draw(SpriteBatch batch) {
            foreach (var fruit in _fruits) {
                fruit.Draw(batch);
            }
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
                if (f.Eaten) {
                    continue;
                }
                float sqDis = f.BoundingBox.SquaredDistanceTo(position);
                if (sqDis < minDis) {
                    closest = f;
                    minDis = sqDis;
                }
            }
            if (closest == null || minDis > range * range) {
                fruit = null;
                return null;
            }
            Vector2 direction = closest.BoundingBox.Center - position;
            fruit = closest;
            return direction;
        }
    }
}

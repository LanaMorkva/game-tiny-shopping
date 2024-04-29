using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;

namespace TinyShopping.Game {

    internal abstract class Fruit {
        protected Vector2 Position => BoundingBox.Position;
        public bool ShouldRemove => _health <= 0;
        public RectangleF BoundingBox { get; protected set; }

        private World _world;

        protected int _health;

        protected Texture2D _texture;
        public Fruit(World world, Texture2D texture) {
            _world = world;
            _texture = texture;
        }

        public abstract void Draw(SpriteBatch _);

        public abstract bool Contains(Rectangle objRect);

        public abstract bool Collides(Polygon objRect);

        public void EatFruit() {
            _health -= 1;
        }
    }

    internal class FruitPiece : Fruit {

        public FruitPiece(RectangleF boundingBox, World world, Texture2D texture) : base(world, texture) {
            BoundingBox = boundingBox;
            _health = 1;
        }

        /// <summary>
        /// Draws the fruit to the sprite batch.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        public override void Draw(SpriteBatch batch) {
            batch.Draw(_texture, BoundingBox.ToRectangle(), Color.White);
        }

        public override bool Contains(Rectangle objRect) {
            return BoundingBox.Intersects(objRect);
        }

        public override bool Collides(Polygon objRect) {
            return false;
        }  
    }

    internal class FruitBox : Fruit {

        private RectangleF _sourceBox;
        private Obstacle _obstacle;
        private int _maxHealth = 5;

        private int _width;
        private int _height;
    
        public FruitBox(Vector2 boxLeftCorner, World world, Texture2D texture) : base(world, texture) {
            _health = _maxHealth;
            _height = _texture.Height/6;
            _width = _texture.Width/3;
            int rowNum = Random.Shared.Next(6);
            int colNum = Random.Shared.Next(3);
            _sourceBox = new RectangleF(_width * colNum, _height * rowNum, _width, _height);
            BoundingBox = new RectangleF(boxLeftCorner - new Vector2(0, _height - Constants.BOX_BOTTOM_H), 
                                            new Size2(_width, _height));
            AddBottomObstacle(boxLeftCorner);
        }

        private void AddBottomObstacle(Vector2 boxLeftCorner) {
            List<Vector2> boxBottomPoints = new List<Vector2>()
            {   
                new(0,0), 
                new(_width / 2, Constants.BOX_BOTTOM_H), 
                new(_width, 0), 
                new(_width / 2, -Constants.BOX_BOTTOM_H)
            };

            _obstacle = new Obstacle(boxLeftCorner, boxBottomPoints);
        }

        /// <summary>
        /// Draws the fruit to the sprite batch.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        public override void Draw(SpriteBatch batch) {
            batch.Draw(_texture, BoundingBox.ToRectangle(), _sourceBox.ToRectangle(), Color.White);

#if DEBUG
            _obstacle.Draw(batch);
#endif

            if (_health < _maxHealth) {
                var barPos = Position.ToPoint() - new Point(0, 8);
                var healthBar = new Rectangle(barPos, new Size((int)(BoundingBox.Width * _health / _maxHealth), 6));
                var healthBarBound = new Rectangle(barPos, new Size((int)BoundingBox.Width, 6));

                batch.FillRectangle(healthBar, Color.Green);
                batch.DrawRectangle(healthBarBound, Color.Black);
            }
        }

        public override bool Contains(Rectangle objRect) {
            return _obstacle.IsColliding(new Polygon(objRect.ToRectangleF().GetCorners()));
        }

        public override bool Collides(Polygon objRect) {
            return _obstacle.IsColliding(objRect);
        }
    }
}

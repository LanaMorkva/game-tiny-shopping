using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLab.TinyShopping {

    internal class Insect {

        private World _world;

        private Texture2D _texture;

        private int _textureSize;

        private Vector2 _position;

        private float _rotationRad;

        private int _rotation;

        private int _rotationTarget;

        private Vector2 _direction;

        private double _nextUpdateTime;

        public Insect(World world) {
            _world = world;
        }

        /// <summary>
        /// Loads necessary data from disk.
        /// </summary>
        /// <param name="contentManager">The content manager of the main game.</param>
        public void LoadContent(ContentManager contentManager) {
            _texture = contentManager.Load<Texture2D>("ant_texture");
            _position = new Vector2(300, 300);
            _textureSize = (int)_world.TileSize;
        }

        /// <summary>
        /// Draws the world to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
            Rectangle destination = new Rectangle((int)_position.X, (int)_position.Y, _textureSize, _textureSize);
            batch.Draw(_texture, destination, null, Color.White, _rotationRad, origin, SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime) {
            if (gameTime.TotalGameTime.TotalMilliseconds > _nextUpdateTime) {
                _nextUpdateTime = gameTime.TotalGameTime.TotalMilliseconds + Random.Shared.Next(5000) + 500;
                _rotationTarget = Random.Shared.Next(360);
                _rotationRad = MathHelper.ToRadians(_rotationTarget);
                _direction = new Vector2(MathF.Sin(_rotationRad), -MathF.Cos(_rotationRad));
                _direction.Normalize();
            }
            if (_rotation != _rotationTarget) {
                int diff = _rotationTarget - _rotation;
                _rotation += diff / Math.Abs(diff);
                _rotationRad = MathHelper.ToRadians(_rotation);
            }
            else {
                _position.X += _direction.X;
                _position.Y += _direction.Y;
            }
        }
    }
}

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

        private static readonly int SPEED = 50;

        private static readonly int ROTATION_SPEED = 100;

        private readonly World _world;

        private Texture2D _texture;

        private int _textureSize;

        private double _nextUpdateTime;

        private InsectPos _position;

        private bool _isTurning;

        public Insect(World world, Vector2 spawn) {
            _world = world;
            _position = new InsectPos((int)spawn.X, (int)spawn.Y);
        }

        /// <summary>
        /// Loads necessary data from disk.
        /// </summary>
        /// <param name="contentManager">The content manager of the main game.</param>
        public void LoadContent(ContentManager contentManager, Texture2D texture) {
            _texture = texture;
            _textureSize = (int)_world.TileSize;
        }

        /// <summary>
        /// Draws the world to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);
            Rectangle destination = new ((int)_position.X, (int)_position.Y, _textureSize, _textureSize);
            batch.Draw(_texture, destination, null, Color.White, _position.Rotation, origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Updates the ant's position and rotation.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            if (_isTurning) {
                RecoverCollision(gameTime);
            }
            else if (!_world.IsWalkable(_position.X, _position.Y, _textureSize / 2)) {
                _isTurning = true;
                _position.TargetRotation += 180;
            }
            else {
                Wander(gameTime);
            }
            
        }

        /// <summary>
        /// Recovers from a collision with the map by backing up and turning.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void RecoverCollision(GameTime gameTime) {
            if (!_position.IsTurning) {
                _isTurning = false;
            }
            if (!_world.IsWalkable(_position.X, _position.Y, _textureSize)) {
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * -SPEED);
            }
            else {
                _position.Rotate((float)gameTime.ElapsedGameTime.TotalSeconds * ROTATION_SPEED);
            }
        }

        /// <summary>
        /// Moves around randomly.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void Wander(GameTime gameTime) {
            if (gameTime.TotalGameTime.TotalMilliseconds > _nextUpdateTime) {
                _nextUpdateTime = gameTime.TotalGameTime.TotalMilliseconds + Random.Shared.Next(5000) + 500;
                _position.TargetRotation = Random.Shared.Next(360);
            }
            if (_position.IsTurning) {
                _position.Rotate((float)gameTime.ElapsedGameTime.TotalSeconds * ROTATION_SPEED);
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * SPEED / 3);
            }
            else {
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * SPEED);
            }
        }
    }
}

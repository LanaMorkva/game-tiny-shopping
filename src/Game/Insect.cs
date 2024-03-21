using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    internal class Insect {

        private static readonly int SPEED = 50;

        private static readonly int ROTATION_SPEED = 100;

        private readonly World _world;

        private Texture2D _texture;

        private Texture2D _textureFull;

        private int _textureSize;

        private double _nextUpdateTime;

        private InsectPos _position;

        private bool _isRecovering;

        private PheromoneHandler _handler;

        private FruitHandler _fruits;

        private bool _isSpawning;

        private bool _isCarrying;

        private Colony _colony;

        public Insect(World world, PheromoneHandler handler, Vector2 spawn, int spawnRotation, FruitHandler fruits, Texture2D texture, Texture2D textureFull, Colony colony) {
            _world = world;
            _handler = handler;
            _position = new InsectPos((int)spawn.X, (int)spawn.Y, spawnRotation);
            _fruits = fruits;
            _isSpawning = true;
            _texture = texture;
            _textureFull = textureFull;
            _textureSize = (int)_world.TileSize;
            _colony = colony;
        }

        /// <summary>
        /// Draws the insect to the sprite batch.
        /// </summary>
        /// <param name="batch">The batch to draw to.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);
            Rectangle destination = new ((int)_position.X, (int)_position.Y, _textureSize, _textureSize);
            batch.Draw(_isCarrying ? _textureFull : _texture, destination, null, Color.White, _position.Rotation, origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Updates the ant's position and rotation.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            // handle spawning
            if (_isSpawning) {
                if (_world.IsWalkable(_position.X, _position.Y, _textureSize / 2)) {
                    _isSpawning = false;
                }
                Walk(gameTime);
                return;
            }
            // handle collision
            if (_isRecovering) {
                RecoverCollision(gameTime);
                return;
            }
            if (!_world.IsWalkable(_position.X, _position.Y, _textureSize / 2)) {
                _isRecovering = true;
                _position.TargetRotation += 180;
                return;
            }
            // handle close-by fruit
            if (!_isCarrying) {
                Vector2? dir = _fruits.GetDirectionToClosestFruit(new Vector2(_position.X, _position.Y), out Fruit closestFruit);
                if (dir != null && dir.Value.LengthSquared() <= (_world.TileSize * _world.TileSize) / 4) {
                    _isCarrying = true;
                    _fruits.RemoveFruit(closestFruit);
                    return;
                }
                if (dir != null) {
                    _position.TargetDirection = dir.Value;
                    Walk(gameTime);
                    return;
                }
            }
            // handle fruit drop off
            if (_isCarrying && Vector2.DistanceSquared(new Vector2(_position.X, _position.Y), _colony.DropOff) < _world.TileSize*_world.TileSize) {
                _isCarrying = false;
                _colony.IncreaseFruitCount();
                return;
            }
            // handle pheromones
            if (!_isCarrying) {
                Vector2? dir = _handler.GetDirectionToForwardPheromone(new Vector2(_position.X, _position.Y));
                if (dir != null) {
                    _position.TargetDirection = dir.Value;
                    Walk(gameTime);
                    return;
                }
            }
            else {
                Vector2? dir = _handler.GetDirectionToReturnPheromone(new Vector2(_position.X, _position.Y));
                if (dir != null) {
                    _position.TargetDirection = dir.Value;
                    Walk(gameTime);
                    return;
                }
            }
            // wander if nothing else was done
            Wander(gameTime);
            
        }

        /// <summary>
        /// Recovers from a collision with the map by backing up and turning.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void RecoverCollision(GameTime gameTime) {
            if (!_position.IsTurning) {
                _isRecovering = false;
            }
            if (!_world.IsWalkable(_position.X, _position.Y, _textureSize/2)) {
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
            Walk(gameTime);
        }

        /// <summary>
        /// Turns towards the target and walks towards it.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void Walk(GameTime gameTime) {
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

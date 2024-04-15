using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TinyShopping.Game.AI;

namespace TinyShopping.Game {

    internal class Insect {

        private enum AnimationKey
        {
            Left,
            Right,
            LeftFull,
            RightFull
        }

        private static readonly int SPEED = 70;

        private static readonly int ROTATION_SPEED = 100;

        private readonly World _world;

        private Texture2D _texture;

        private Texture2D _textureFull; // legacy

        public int TextureSize { get; private set; }

        private double _nextUpdateTime;

        private InsectPos _position;

        private readonly AnimationManager _animationManager = new();

        public bool IsCarrying { get; set; }

        public int Owner { get; private set; }

        public Vector2 Position {
            get {
                return _position.Position;
            }
        }

        public int TargetRotation {
            get {
                return _position.TargetRotation;
            }
            set {
                _position.TargetRotation = value;
            }
        }

        public bool IsTurning {
            get {
                return _position.IsTurning;
            }
        }

        public Vector2 TargetDirection {
            set {
                _position.TargetDirection = value;
            }
        }

        public int Health { private set; get; }

        private Task[] _ais;

        public Insect(World world, PheromoneHandler handler, Vector2 spawn, int spawnRotation, FruitHandler fruits, Texture2D texture, Texture2D textureFull, Colony colony, int owner) {
            _world = world;
            _position = new InsectPos((int)spawn.X, (int)spawn.Y, spawnRotation);
            _texture = texture;
            _textureFull = textureFull;
            TextureSize = (int)_world.TileWidth / 2;
            Owner = owner;
            Health = 100;
            _ais = new Task[] {
                new Spawn(this, _world),
                new Collide(this, _world),
                new PickUp(this, _world, fruits),
                new DropOff(this, _world, colony),
                new FollowPheromone(this, _world, handler, colony),
            };

            _animationManager.AddAnimation(AnimationKey.Left, new Animation(_texture, 2, 4, 0.2f, 1));
            _animationManager.AddAnimation(AnimationKey.Right, new Animation(_texture, 2, 4, 0.2f, 2));
            _animationManager.AddAnimation(AnimationKey.LeftFull, new Animation(_texture, 2, 4, 0.2f, 3));
            _animationManager.AddAnimation(AnimationKey.RightFull, new Animation(_texture, 2, 4, 0.2f, 4));
        }

        /// <summary>
        /// Draws the insect to the sprite batch.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current time information.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            //Texture2D texture = IsCarrying ? _textureFull : _texture;
            Rectangle destination = new Rectangle(_position.X, _position.Y, TextureSize, TextureSize);
            Vector2 origin = new Vector2(_texture.Width / 4f, _texture.Height / 8f);
            //batch.Draw(texture, destination, null, Color.White, _position.Rotation, origin, SpriteEffects.None, 0);

            _animationManager.Draw(batch, gameTime, destination, origin);
        }

        /// <summary>
        /// Updates the ant's position and rotation.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            UpdateAnimationManager(gameTime);
            foreach (var ai in _ais) {
                if (ai.Run(gameTime)) {
                    
                    return;
                }
            }
            Wander(gameTime);
        }

        /// <summary>
        /// Updates the animation manager.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void UpdateAnimationManager(GameTime gameTime)
        {
            // TODO: fix idle state, fix rotation, add more rotation states
            
            bool movesLeft = _position.TargetRotation <= 180;
            if (movesLeft && !IsCarrying)
            {
                _animationManager.Update(AnimationKey.Left, gameTime);
            }
            else
            if (!movesLeft && !IsCarrying)
            {
                _animationManager.Update(AnimationKey.Right, gameTime);
            }
            else
            if (movesLeft && IsCarrying)
            {
                _animationManager.Update(AnimationKey.LeftFull, gameTime);
            }
            else
            if (!movesLeft && IsCarrying)
            {
                _animationManager.Update(AnimationKey.RightFull, gameTime);
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
        public void Walk(GameTime gameTime) {
            if (_position.IsTurning) {
                _position.Rotate((float)gameTime.ElapsedGameTime.TotalSeconds * ROTATION_SPEED);
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * SPEED / 3);
            }
            else if (IsCarrying) {
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * SPEED * 3/5);
            }
            else {
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * SPEED);
            }
        }

        /// <summary>
        /// Rotates the ant without moving forward.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Rotate(GameTime gameTime) {
            _position.Rotate((float)gameTime.ElapsedGameTime.TotalSeconds * ROTATION_SPEED);
        }

        /// <summary>
        /// Moves backwards.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void BackUp(GameTime gameTime) {
            _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * -SPEED);
        }

        /// <summary>
        /// Reduces the insect's health.
        /// </summary>
        /// <param name="damage">The damage to take.</param>
        public void TakeDamage(int damage) {
            Health -= damage;
        }
    }
}

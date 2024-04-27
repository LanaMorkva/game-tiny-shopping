using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using TinyShopping.Game.AI;
using TinyShopping.Game.Pathfinding;
using PFPoint = TinyShopping.Game.Pathfinding.Point;

namespace TinyShopping.Game {

    internal struct Services {
        public World world;
        public PheromoneHandler handler;
        public FruitHandler fruits;
        public Colony colony;
    }

    internal struct Attributes {
        public int speed;
        public int rotationSpeed;
        public int maxHealth;
        public int damage;
    }

    internal class Insect {

        private enum AnimationKey {
            Left,
            Right,
            LeftFull,
            RightFull
        }

        private readonly World _world;

        private Texture2D _texture;

        public int TextureSize { get; private set; } // equal to Constants.ANT_TEXTURE_SIZE, can be removed

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

        private Attributes _attributes;

        public int Health { private set; get; }

        public int Damage { 
            get {
                return _attributes.damage;
            } 
        }

        private Task[] _ais;

        private Pathfinder _pathFinder;

        private IList<PFPoint> _path = new List<PFPoint>();

        private int _pathIndex;

        private Vector2 _target;

        public Insect(Services services, Vector2 spawn, int spawnRotation, Texture2D texture, int owner, Attributes attributes) {
            _world = services.world;
            _pathFinder = new Pathfinder(_world);
            _position = new InsectPos((int)spawn.X, (int)spawn.Y, spawnRotation);
            _texture = texture;
            _attributes = attributes;
            TextureSize = Constants.ANT_TEXTURE_SIZE;
            Owner = owner;
            Health = attributes.maxHealth;
            _ais = new Task[] {
                new Spawn(this, _world),
                new Collide(this, _world),
                new PickUp(this, _world, services.fruits),
                new DropOff(this, _world, services.colony),
                new FollowPheromone(this, _world, services.handler, services.colony),
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
            Rectangle destination = new Rectangle(_position.X - TextureSize / 2, _position.Y - TextureSize / 2, TextureSize, TextureSize);
            Vector2 origin = new Vector2(0, 0);
            //batch.Draw(texture, destination, null, Color.White, _position.Rotation, origin, SpriteEffects.None, 0);

            _animationManager.Draw(batch, gameTime, destination, origin);

#if DEBUG
            batch.DrawRectangle(destination, Color.Red);
            batch.DrawLine(Position, 30, _position.Rotation - (float) Math.PI/2, Color.Blue);
            batch.DrawLine(Position, 30, MathHelper.ToRadians(_position.TargetRotation - 90), Color.Red);
            Vector2 current = Position;
            for (int i = _pathIndex; i < _path.Count; i++) {
                Vector2 p = new Vector2(_path[i].X, _path[i].Y);
                batch.DrawLine(current, p, Color.Black, 2);
                current = p;
            }
#endif
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
        private void UpdateAnimationManager(GameTime gameTime) {
            // TODO: fix idle state, fix rotation, add more rotation states
            
            bool movesLeft = _position.Rotation <= Math.PI;
            if (movesLeft && !IsCarrying) {
                _animationManager.Update(AnimationKey.Left, gameTime);
            } else  if (!movesLeft && !IsCarrying) {
                _animationManager.Update(AnimationKey.Right, gameTime);
            } else if (movesLeft && IsCarrying) {
                _animationManager.Update(AnimationKey.LeftFull, gameTime);
            } else if (!movesLeft && IsCarrying) {
                _animationManager.Update(AnimationKey.RightFull, gameTime);
            }
        }

        /// <summary>
        /// Moves around randomly.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void Wander(GameTime gameTime) {
            if (_path.Count > 0) {
                _path = new List<PFPoint>();
            }
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
                _position.Rotate((float)gameTime.ElapsedGameTime.TotalSeconds * _attributes.rotationSpeed);
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * _attributes.speed / 3);
            }
            else if (IsCarrying) {
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * _attributes.speed * 3/5);
            }
            else {
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * _attributes.speed);
            }
        }

        /// <summary>
        /// Walks towards the given target.
        /// </summary>
        /// <param name="target">The target to walk to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void WalkTo(Vector2 target, GameTime gameTime) {
            if (Vector2.DistanceSquared(target, _target) > 32) {
                _target = target;
                _path = _pathFinder.FindPath(Position, target);
                _pathIndex = 0;
            }
            if (_pathIndex >= _path.Count) {
                Wander(gameTime);
                return;
            }
            Vector2 nextPoint = new Vector2(_path[_pathIndex].X, _path[_pathIndex].Y);
            TargetDirection = nextPoint - Position;
            Walk(gameTime);
            if (Vector2.DistanceSquared(nextPoint, Position) < 256) {
                _pathIndex++;
            }
        }

        /// <summary>
        /// Rotates the ant without moving forward.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Rotate(GameTime gameTime) {
            _position.Rotate((float)gameTime.ElapsedGameTime.TotalSeconds * _attributes.rotationSpeed);
        }

        /// <summary>
        /// Moves backwards.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void BackUp(GameTime gameTime) {
            _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * -_attributes.speed);
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Linq;
using TinyShopping.Game.AI;

namespace TinyShopping.Game {
    enum InsectState {
        Wander = 0, 
        Run = 1,
        CarryWander = 2,
        CarryRun = 3,

        FightRun = 4,
        FightWander = 5,
        // active fight
        Fight = 6,
    }

    internal struct Services {
        public World world;
        public PheromoneHandler handler;
        public FruitHandler fruits;
        public Colony colony;
        public InsectHandler coloniesHandler;
    }

    internal struct Attributes {
        public int speed;
        public int rotationSpeed;
        public int maxHealth;
        public int damage;
        public int damageReload;
    }

    internal class Insect {

        private enum AnimationKey {
            Left,
            Right,
            LeftFull,
            RightFull
        }

        private readonly World _world;

        private InsectHandler _coloniesHandler;

        private Texture2D _texture;

        public int TextureSize { get; private set; } // equal to Constants.ANT_TEXTURE_SIZE, can be removed

        private InsectPos _position;

        private readonly AnimationManager _animationManager = new();

        private InsectState _state;

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

        public bool IsWandering {
            get {
                return _aiHandler.IsWandering;
            }
        }

        public Vector2 TargetDirection {
            set {
                _position.TargetDirection = value;
            }
        }

        private readonly Attributes _attributes;
        private readonly ColonyType _insectType;

        public int Health { private set; get; }

        public bool CanGiveDamage { 
            get {
                return _damageCooldown <= 0;
            }
        }

        public int GiveDamage { 
            get {
                _damageCooldown = _attributes.damageReload;
                return _attributes.damage;
            } 
        }

        private float _pheromoneCooldown;
        private float _damageCooldown = 0;

        private PheromoneHandler _pheromoneHandler;

        private bool _wasCarrying;

        private int _pheromonePriority = 100;


        private AIHandler _aiHandler;

        public Insect(Services services, Vector2 spawn, int spawnRotation, Texture2D texture, ColonyType insectType) {
            _world = services.world;
            _pheromoneHandler = services.handler;
            _coloniesHandler = services.coloniesHandler;
            _position = new InsectPos((int)spawn.X, (int)spawn.Y, spawnRotation);
            _texture = texture;
            _insectType = insectType;
            _attributes = insectType == ColonyType.ANT ? Constants.ANT_ATTRIBUTES : Constants.TERMITE_ATTRIBUTES;
            TextureSize = Constants.ANT_TEXTURE_SIZE;
            Owner = (int)insectType;
            Health = _attributes.maxHealth;
            _aiHandler = new AIHandler(this, services);

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
        public void Draw(SpriteBatch batch, GameTime gameTime, bool playersInsect) {
            Rectangle destination = new Rectangle(_position.X - TextureSize / 2, _position.Y - TextureSize / 2, TextureSize, TextureSize);
            Vector2 origin = new Vector2(0, 0);
            _animationManager.Draw(batch, gameTime, destination, origin);

            var barPos = Position.ToPoint() - new Point2(TextureSize / 2, 8);
            var healthBar = new RectangleF(barPos, new Size2(destination.Width * Health / _attributes.maxHealth, 3));
            var healthBarBound = new RectangleF(barPos, new Size2(destination.Width, 3));

            batch.FillRectangle(healthBar, playersInsect ? Color.Green : Color.Red);
            batch.DrawRectangle(healthBarBound, Color.Black);

#if DEBUG
            var color = Color.White;
            switch(_state) {
                case InsectState.Run: color = Color.Green; break;
                case InsectState.Wander: color = Color.LightGreen; break;
                case InsectState.CarryRun: color = Color.Blue; break;
                case InsectState.CarryWander: color = Color.LightBlue; break;
                case InsectState.Fight: color = Color.Red; break;
                case InsectState.FightWander:
                case InsectState.FightRun: color = Color.Pink; break;
            };
            batch.DrawRectangle(destination, color);
            batch.DrawLine(Position, 30, _position.Rotation - (float) Math.PI/2, Color.Blue);
            batch.DrawLine(Position, 30, MathHelper.ToRadians(_position.TargetRotation - 90), Color.Red);
#endif
        }

        /// <summary>
        /// Updates the ant's position and rotation.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            _pheromoneCooldown -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_pheromoneCooldown <= 0) {
                if (IsCarrying != _wasCarrying) {
                    _pheromonePriority = Constants.TRAIL_PHEROMONE_START_PRIORITY;
                }
                PheromoneType type = IsCarrying ? PheromoneType.DISCOVER : PheromoneType.RETURN;
                if (_pheromonePriority > 0) {
                    _pheromoneHandler.AddPheromone(Position, gameTime, type, Owner, _pheromonePriority--, 30000, 32, false);
                }
                _pheromoneCooldown = 250;
                _wasCarrying = IsCarrying;
            }
            if (_damageCooldown > 0) {
                _damageCooldown -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            UpdateAnimationManager(gameTime);
            _aiHandler.RunNextTask(gameTime);
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
        /// Turns towards the target and walks towards it.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Walk(GameTime gameTime, InsectState state) {
            _state = state;
            if (_position.IsTurning) {
                _position.Rotate((float)gameTime.ElapsedGameTime.TotalSeconds * _attributes.rotationSpeed);
                _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * _attributes.speed / 3);
                return;
            }
            float speed = _attributes.speed;
            switch (_state) {
                case InsectState.FightWander:
                case InsectState.CarryWander:
                case InsectState.Wander: speed = Constants.WANDER_SPEED; break;
                case InsectState.CarryRun: {
                    if (_insectType == ColonyType.ANT) {
                        speed = _attributes.speed * 3/5;
                    } else {
                        speed = _attributes.speed * 4/5;
                    }
                    break;
                }
            }
            _position.Move((float)gameTime.ElapsedGameTime.TotalSeconds * speed);
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

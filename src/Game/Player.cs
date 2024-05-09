using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace TinyShopping.Game {

    /// <summary>
    /// The Player handles user input and pheromone placement
    /// </summary>
    internal class Player {

        private Vector2 _position;
        private Vector2 _targetPosition;
        private int _cursorSize;

        private Texture2D _texture;

        private int _discoverPressed;

        private int _returnPressed;

        private int _fightPressed;

        private bool _newInsectPressed;

        private PheromoneHandler _handler;

        private PlayerInput _input;

        private InsectHandler _insectHandler;

        private World _world;

        private int _id;

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="handler">The pheromone handler to use.</param>
        /// <param name="input">The player input to use.</param>
        /// <param name="insects">The insect handler to use..</param>
        /// <param name="id">The palyer id, 0 or 1.</param>
        /// <param name="position">The cursor starting position</param>
        public Player(PheromoneHandler handler, PlayerInput input, InsectHandler insects, World world, int id, Vector2 position) {
            _position = position;
            _handler = handler;
            _input = input;
            _insectHandler = insects;
            _world = world;
            _id = id;
        }

        /// <summary>
        /// Loads the necessary files from disk.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content) {
            _texture = content.Load<Texture2D>("crosshair");
        }

        /// <summary>
        /// Draws the player cursor.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            int size = _cursorSize;
            Rectangle destination = new Rectangle((int)(_position.X - size / 2f), (int)(_position.Y - size / 2f), size, size);
            batch.Draw(_texture, destination, Color.White);
            int pressDuration = _discoverPressed + _fightPressed + _returnPressed;
            if (pressDuration > 0) {
                int range = (int)(Constants.PHEROMONE_RANGE + Constants.PHEROMONE_RANGE_COEFFICIENT * pressDuration / 1000f);
                CircleF circle = new CircleF(_position, MathHelper.Min(range, Constants.PHEROMONE_MAX_RANGE));
                batch.DrawCircle(circle, 100, Color.DarkGray, 3f);
            }
        }

        /// <summary>
        /// Reads user input and updates the cursor position.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="handler">The split screen handler to use.</param>
        public void Update(GameTime gameTime, SplitScreenHandler handler, Scene scene) {
            int speed = (int)(gameTime.ElapsedGameTime.TotalSeconds * Constants.CURSOR_SPEED / handler.GetZoomValue(_id));
            if (_input.IsStartedPressed()) {
                scene.IsPaused = true;
            }
            PlacePheromones(gameTime);
            UpdatePosition(speed);
            ClipCursorToWorld();
            handler.UpdateCameraState(_id, _position, speed, _input.GetZoom(), _input.GetCameraMotion());

            _cursorSize = (int)(Constants.CURSOR_SIZE / handler.GetZoomValue(_id));
        }

        public void SetCursorTo(Vector2 position) {
            _position = position;
        }

        /// <summary>
        /// Clip the cursor position to the world.
        /// </summary>
        private void ClipCursorToWorld() {
            int size = _cursorSize;
            Rectangle cursor = new Rectangle((int)(_position.X - size / 2f), (int)(_position.Y - size / 2f), size, size);
            Rectangle worldRect = _world.GetWorldBoundary();
            if (cursor.X < worldRect.Left) {
                _position.X = worldRect.Left + size / 2f;
            }
            if (cursor.Right > worldRect.Right) {
                _position.X = worldRect.Right - size / 2f;
            }
            if (cursor.Y < worldRect.Top) {
                _position.Y = worldRect.Top + size / 2f;
            }
            if (cursor.Bottom > worldRect.Bottom) {
                _position.Y = worldRect.Bottom - size / 2f;
            }
        }

        /// <summary>
        /// Updates the cursor's position according to player input.
        /// </summary>
        /// <param name="speed">The speed to use.</param>
        private void UpdatePosition(int speed) {
            Vector2 motion = _input.GetMotion();
            _targetPosition.X += motion.X * speed;
            _targetPosition.Y += motion.Y * speed;

            Vector2 lerped = Vector2.Lerp(Vector2.Zero, _targetPosition, 0.8f);
            _position += lerped;
            _targetPosition -= lerped;
        }

        /// <summary>
        /// Places pheromones and exchanges fruits for ants according to player input.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        private void PlacePheromones(GameTime gameTime) {
            if (_input.IsDiscoverPressed()) {
                _discoverPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            else if (_discoverPressed > 0) {
                float range = Constants.PHEROMONE_RANGE + Constants.PHEROMONE_RANGE_COEFFICIENT * _discoverPressed / 1000f;
                _handler.AddPheromone(_position, gameTime, PheromoneType.DISCOVER, _id, Constants.PHEROMONE_MAX_PRIORITY, 
                                    Constants.PHEROMONE_DURATION, MathHelper.Min((int) range, Constants.PHEROMONE_MAX_RANGE), true);
                _discoverPressed = 0;
            }
            if (_input.IsReturnPressed()) {
                _returnPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds); ;
            }
            else if (_returnPressed > 0) {
                float range = Constants.PHEROMONE_RANGE + Constants.PHEROMONE_RANGE_COEFFICIENT * _returnPressed / 1000f;
                _handler.AddPheromone(_position, gameTime, PheromoneType.RETURN, _id, Constants.PHEROMONE_MAX_PRIORITY,
                                    Constants.PHEROMONE_DURATION, MathHelper.Min((int)range, Constants.PHEROMONE_MAX_RANGE), true);
                _returnPressed = 0;
            }
            if (_input.IsFightPressed()) {
                _fightPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds); ;
            }
            else if (_fightPressed > 0) {
                float range = Constants.PHEROMONE_RANGE + Constants.PHEROMONE_RANGE_COEFFICIENT * _fightPressed / 1000f;
                _handler.AddPheromone(_position, gameTime, PheromoneType.FIGHT, _id, Constants.PHEROMONE_MAX_PRIORITY,
                                    Constants.PHEROMONE_DURATION, MathHelper.Min((int)range, Constants.PHEROMONE_MAX_RANGE), true);
                _fightPressed = 0;
            }
            if (_input.IsNewInsectPressed()) {
                _newInsectPressed = true;
            }
            else if (_newInsectPressed) {
                _newInsectPressed = false;
                _insectHandler.BuyNewInsect(_id);
            }
        }
    }
}

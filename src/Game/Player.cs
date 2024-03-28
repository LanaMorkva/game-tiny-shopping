using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping.Game {

    /// <summary>
    /// The Player handles user input and pheromone placement
    /// </summary>
    internal class Player {

        private static readonly int SIZE = 50;

        private static readonly int SPEED = 200;

        private Vector2 _position;

        private Texture2D _texture;

        private int _discoverPressed;

        private int _returnPressed;

        private int _fightPressed;

        private bool _newInsectPressed;

        private PheromoneHandler _handler;

        private PlayerInput _input;

        private InsectHandler _insectHandler;

        private int _id;

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="handler">The pheromone handler to use.</param>
        /// <param name="input">The player input to use.</param>
        /// <param name="insects">The insect handler to use..</param>
        /// <param name="id">The palyer id, 0 or 1.</param>
        /// <param name="position">The cursor starting position</param>
        public Player(PheromoneHandler handler, PlayerInput input, InsectHandler insects, int id, Vector2 position) {
            _position = position;
            _handler = handler;
            _input = input;
            _insectHandler = insects;
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
        public void Draw(SplitScreenHandler handler, GameTime gameTime) {
            Rectangle destination = new Rectangle((int)(_position.X - SIZE / 2f), (int)(_position.Y - SIZE / 2f), SIZE, SIZE);
            handler.RenderObject(_texture, destination, _id);
        }

        /// <summary>
        /// Reads user input and updates the cursor position.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="handler">The split screen handler to use.</param>
        public void Update(GameTime gameTime, SplitScreenHandler handler) {
            int speed = (int)(gameTime.ElapsedGameTime.TotalSeconds * SPEED);
            PlacePheromones(gameTime);
            UpdatePosition(speed);
            handler.UpdateCameraPosition(_id, _position, speed);
            ClipToCamera(handler);
        }

        /// <summary>
        /// Updates the position to make sure the cursor stays within the player camera's view.
        /// </summary>
        /// <param name="handler">The split screen handler to use.</param>
        private void ClipToCamera(SplitScreenHandler handler) {
            Rectangle cursor = new Rectangle((int)(_position.X - SIZE / 2f), (int)(_position.Y - SIZE / 2f), SIZE, SIZE);
            Rectangle camera = handler.GetPlayerCameraBounds(_id);
            if (cursor.X < camera.X) {
                _position.X = camera.X + SIZE / 2f;
            }
            if (cursor.X + cursor.Width > camera.X + camera.Width) {
                _position.X = camera.X + camera.Width - SIZE / 2f;

            }
            if (cursor.Y < camera.Y) {
                _position.Y = camera.Y + SIZE / 2f;
            }
            if (cursor.Y + cursor.Height > camera.Y + camera.Height) {
                _position.Y = camera.Y + camera.Height - SIZE / 2f;
            }
        }

        /// <summary>
        /// Updates the cursor's position according to player input.
        /// </summary>
        /// <param name="speed">The speed to use.</param>
        private void UpdatePosition(int speed) {
            Vector2 motion = _input.GetMotion();
            _position.X += motion.X * speed;
            _position.Y += motion.Y * speed;
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
                _handler.AddPheromone(_position, gameTime, PheromoneType.DISCOVER, _id, 5000 + _discoverPressed * 2);
                _discoverPressed = 0;
            }
            if (_input.IsReturnPressed()) {
                _returnPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds); ;
            }
            else if (_returnPressed > 0) {
                _handler.AddPheromone(_position, gameTime, PheromoneType.RETURN, _id, 5000 + _returnPressed * 2);
                _returnPressed = 0;
            }
            if (_input.IsFightPressed()) {
                _fightPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds); ;
            }
            else if (_fightPressed > 0) {
                _handler.AddPheromone(_position, gameTime, PheromoneType.FIGHT, _id, 5000 + _fightPressed * 2);
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

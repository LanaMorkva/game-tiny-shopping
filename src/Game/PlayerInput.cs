using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace TinyShopping.Game {
    internal class Control {
        public Texture2D Texture { get; }
        public String Description { get; }
        public Rectangle SourceRect { get; }

        public Control(Texture2D texture, Rectangle source, String description) {
            Texture = texture;
            SourceRect = source;
            Description = description;
        }
    }

    internal abstract class PlayerInput {

        protected PlayerIndex _playerIndex;
        public List<Control> Controls { get; protected set; }

        /// <summary>
        /// Reads the desired player motion from the input.
        /// </summary>
        /// <returns>A Vector2 of motion input.</returns>
        /// <exception cref="Exception">Thrown if no input was received.</exception>
        public abstract Vector2 GetMotion();

        /// <summary>
        /// Reads the desired player camera motion from the input.
        /// </summary>
        /// <returns>A Vector2 of motion input.</returns>
        /// <exception cref="Exception">Thrown if no input was received.</exception>
        public abstract Vector2 GetCameraMotion();

        public abstract float GetZoom();

        /// <summary>
        /// Checks if the discover pheromone button is pressed.
        /// </summary>
        /// <returns>True if the button is pressed, false otherwise.</returns>
        public abstract bool IsDiscoverPressed();

        /// <summary>
        /// Checks if the return pheromone button is pressed.
        /// </summary>
        /// <returns>True if the button is pressed, false otherwise.</returns>
        public abstract bool IsReturnPressed();

        /// <summary>
        /// Checks if the fight pheromone button is pressed.
        /// </summary>
        /// <returns>True if the button is pressed, false otherwise.</returns>
        public abstract bool IsFightPressed();

        /// <summary>
        /// Checks if the new insect button is pressed.
        /// </summary>
        /// <returns>True if the button is pressed, false otherwise.</returns>
        public abstract bool IsNewInsectPressed();

        public abstract bool IsStartedPressed();
        
        public abstract bool IsPlayerKeyboard();
    }

    internal class GamePadInput : PlayerInput {

        private Buttons _discoverButton = Buttons.A;

        private Buttons _returnButton = Buttons.X;

        private Buttons _fightButton = Buttons.B;

        private Buttons _newInsectButton = Buttons.Y;

        private Buttons _startButton = Buttons.Start;

        public GamePadInput(PlayerIndex playerIndex, ContentManager content) {
            _playerIndex = playerIndex;
            var gamePadButtons = content.Load<Texture2D>("stats/controller");
            var buttonSize = new Point(gamePadButtons.Width / 2, gamePadButtons.Height / 2);
            Controls = new List<Control>{
                new (gamePadButtons, new Rectangle(Point.Zero, buttonSize), "Discover"), 
                new (gamePadButtons, new Rectangle(new Point(0, buttonSize.Y), buttonSize), "Return home"), 
                new (gamePadButtons, new Rectangle(new Point(buttonSize.X, 0), buttonSize), "Fight"),  
                new (gamePadButtons, new Rectangle(new Point(buttonSize.X, buttonSize.Y), buttonSize), "Buy new insect"), 
            };
        }

        public override Vector2 GetMotion() {
            GamePadState state = GamePad.GetState(_playerIndex);
            if (!state.IsConnected) {
                throw new Exception("Unable to read player input, no available input methods left!");
            }
            Vector2 motion = state.ThumbSticks.Left;
            motion.Y *= -1;
            return motion;
        }

        public override float GetZoom() {
            GamePadState state = GamePad.GetState(_playerIndex);
            float zoom_change = state.Triggers.Left - state.Triggers.Right;
            return zoom_change * -1 * Constants.ZOOM_SPEED;
        }

        public override Vector2 GetCameraMotion() {
            GamePadState state = GamePad.GetState(_playerIndex);
            if (!state.IsConnected) {
                throw new Exception("Unable to read player input, no available input methods left!");
            }
            Vector2 motion = state.ThumbSticks.Right;
            motion.Y *= -1;
            return motion;
        }


        public override bool IsDiscoverPressed() {
            GamePadState cState = GamePad.GetState(_playerIndex);
            return cState.IsButtonDown(_discoverButton);
        }

        public override bool IsReturnPressed() {
            GamePadState cState = GamePad.GetState(_playerIndex);
            return cState.IsButtonDown(_returnButton);
        }

        public override bool IsFightPressed() {
            GamePadState cState = GamePad.GetState(_playerIndex);
            return cState.IsButtonDown(_fightButton);
        }

        public override bool IsNewInsectPressed() {
            GamePadState cState = GamePad.GetState(_playerIndex);
            return cState.IsButtonDown(_newInsectButton);
        }

        public override bool IsStartedPressed()
        {
            GamePadState cState = GamePad.GetState(_playerIndex);
            return cState.IsButtonDown(_startButton);
        }

        public override bool IsPlayerKeyboard()
        {
            return false;
        }
    }

    internal class KeyboardInput : PlayerInput {

        private Keys _up = Keys.W;

        private Keys _down = Keys.S;

        private Keys _left = Keys.A;

        private Keys _right = Keys.D;

        private Keys _discoverKey = Keys.D1;

        private Keys _returnKey = Keys.D2;

        private Keys _fightKey = Keys.D3;

        private Keys _newInsectKey = Keys.D4;

        private Keys _startKey = Keys.Escape;

        private Keys _zoomIn = Keys.Q;
        private Keys _zoomOut = Keys.E;

        public KeyboardInput(PlayerIndex playerIndex, ContentManager content) {
            if (playerIndex < 0 || playerIndex > PlayerIndex.Two) {
                throw new Exception("Invalid player index, maximum 2 players are allowed");
            }
            var keyboardButtons = content.Load<Texture2D>("stats/numbers");
            var buttonSize = new Point(keyboardButtons.Width / 4, keyboardButtons.Height / 2);
            if (playerIndex == PlayerIndex.Two) {
                _up = Keys.I;
                _down = Keys.K;
                _left = Keys.J;
                _right = Keys.L;
                _zoomIn = Keys.U;
                _zoomOut = Keys.O;

                _discoverKey = Keys.D7;
                _returnKey = Keys.D8;
                _fightKey = Keys.D9;
                _newInsectKey = Keys.D0;
                Controls = new List<Control>{
                    new (keyboardButtons, new Rectangle(new Point(buttonSize.X*2, 0), buttonSize), "Discover"), 
                    new (keyboardButtons, new Rectangle(new Point(buttonSize.X*3, 0), buttonSize), "Return home"), 
                    new (keyboardButtons, new Rectangle(new Point(buttonSize.X*2, buttonSize.Y), buttonSize), "Fight"),  
                    new (keyboardButtons, new Rectangle(new Point(buttonSize.X*3, buttonSize.Y), buttonSize), "Buy new insect"), 
                };
            } else {
                Controls = new List<Control>{
                    new (keyboardButtons, new Rectangle(Point.Zero, buttonSize), "Discover"), 
                    new (keyboardButtons, new Rectangle(new Point(buttonSize.X, 0), buttonSize), "Return home"), 
                    new (keyboardButtons, new Rectangle(new Point(0, buttonSize.Y), buttonSize), "Fight"),  
                    new (keyboardButtons, new Rectangle(new Point(buttonSize.X, buttonSize.Y), buttonSize), "Buy new insect"), 
                };
            }
            _playerIndex = playerIndex;
        }


        public override Vector2 GetMotion() {
            KeyboardState state = Keyboard.GetState();
            Vector2 motion = new Vector2(0, 0);
            if (state.IsKeyDown(_left)) {
                motion.X = -1;
            }
            if (state.IsKeyDown(_right)) {
                motion.X = +1;
            }
            if (state.IsKeyDown(_up)) {
                motion.Y = -1;
            }
            if (state.IsKeyDown(_down)) {
                motion.Y = +1;
            }
            if (motion.LengthSquared() > 0) {
                motion.Normalize();
            }
            return motion;
        }

        public override Vector2 GetCameraMotion()
        {
            return new Vector2(0, 0);
        }

        public override float GetZoom() {
            KeyboardState state = Keyboard.GetState();
            float zoom = 0f;
            if (state.IsKeyDown(_zoomIn)) {
                zoom += Constants.ZOOM_SPEED;
            }
            if (state.IsKeyDown(_zoomOut)) {
                zoom -= Constants.ZOOM_SPEED;
            }
            return zoom;
        }

        public override bool IsDiscoverPressed() {
            KeyboardState kState = Keyboard.GetState();
            return kState.IsKeyDown(_discoverKey);
        }

        public override bool IsReturnPressed() {
            KeyboardState kState = Keyboard.GetState();
            return kState.IsKeyDown(_returnKey);
        }

        public override bool IsFightPressed() {
            KeyboardState kState = Keyboard.GetState();
            return kState.IsKeyDown(_fightKey);
        }

        public override bool IsNewInsectPressed() {
            KeyboardState kState = Keyboard.GetState();
            return kState.IsKeyDown(_newInsectKey);
        }

        public override bool IsStartedPressed()
        {
            KeyboardState kState = Keyboard.GetState();
            return kState.IsKeyDown(_startKey);
        }

        public override bool IsPlayerKeyboard()
        {
            return true;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    internal class PlayerInput {

        private Keys _up;

        private Keys _down;

        private Keys _left;

        private Keys _right;

        private Keys _discoverKey;

        private Keys _returnKey;

        private bool _supportsKeyboard;

        private PlayerIndex _playerIndex;

        private Buttons _discoverButton;

        private Buttons _returnButton;

        private bool _supportsController;

        /// <summary>
        /// Creates an instance that supports keyboard input only.
        /// </summary>
        /// <param name="up">The up button to use.</param>
        /// <param name="down">The down button to use.</param>
        /// <param name="left">The left button to use.</param>
        /// <param name="right">The right button to use.</param>
        /// <param name="discoverPher">The button to place the discover pheromone.</param>
        /// <param name="returnPher">The button to place the return pheromone.</param>
        public PlayerInput(Keys up, Keys down, Keys left, Keys right, Keys discoverPher, Keys returnPher) {
            _up = up;
            _down = down;
            _left = left;
            _right = right;
            _discoverKey = discoverPher;
            _returnKey = returnPher;
            _supportsKeyboard = true;
        }

        /// <summary>
        /// Creates an instance that supports controller input only.
        /// </summary>
        /// <param name="controllerIndex">The controller index.</param>
        /// <param name="discoverPher">The button to place the discover pheromone.</param>
        /// <param name="returnPher">The button to place the return pheromone.</param>
        public PlayerInput(PlayerIndex controllerIndex, Buttons discoverPher, Buttons returnPher) {
            _playerIndex = controllerIndex;
            _discoverButton = discoverPher;
            _returnButton = returnPher;
            _supportsController = true;
        }

        /// <summary>
        /// Creates an instance that supports keyboard and controller input.
        /// </summary>
        /// <param name="up">The up button to use.</param>
        /// <param name="down">The down button to use.</param>
        /// <param name="left">The left button to use.</param>
        /// <param name="right">The right button to use.</param>
        /// <param name="discoverKey">The button to place the discover pheromone.</param>
        /// <param name="returnKey">The button to place the return pheromone.</param>
        /// <param name="playerIndex"></param>
        /// <param name="discoverButton"></param>
        /// <param name="returnButton"></param>
        /// <param name="playerIndex">The controller index.</param>
        /// <param name="discoverButton">The button to place the discover pheromone.</param>
        /// <param name="returnButton">The button to place the return pheromone.</param>
        public PlayerInput(Keys up, Keys down, Keys left, Keys right, Keys discoverKey, Keys returnKey, PlayerIndex playerIndex, Buttons discoverButton, Buttons returnButton) {
            _up = up;
            _down = down;
            _left = left;
            _right = right;
            _discoverKey = discoverKey;
            _returnKey = returnKey;
            _playerIndex = playerIndex;
            _discoverButton = discoverButton;
            _returnButton = returnButton;
            _supportsController = true;
            _supportsKeyboard = true;
        }

        /// <summary>
        /// Reads the desired player motion from the input.
        /// </summary>
        /// <returns>A Vector2 of motion input.</returns>
        /// <exception cref="Exception">Thrown if no input was received.</exception>
        public Vector2 GetMotion() {
            Vector2? motion = null;
            if (_supportsController) {
                motion = GetControllerMotion();
            }
            if ((motion == null || motion.Value.LengthSquared() == 0) && _supportsKeyboard) {
                motion = GetKeyboardMotion();
            }
            if (motion == null) {
                throw new Exception("Unable to read player input, no available input methods left!");
            }
            return motion.Value;
        }

        /// <summary>
        /// Reads the motion from the controller.
        /// </summary>
        /// <returns>A Vector2 with the current motion direction.</returns>
        private Vector2? GetControllerMotion() {
            GamePadState state = GamePad.GetState(_playerIndex);
            if (!state.IsConnected) {
                return null;
            }
            Vector2 motion = state.ThumbSticks.Left;
            motion.Y *= -1;
            return motion;
        }

        /// <summary>
        /// Reads the motion from the keyboard.
        /// </summary>
        /// <returns>A Vector2 with the current motion direction.</returns>
        private Vector2 GetKeyboardMotion() {
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

        /// <summary>
        /// Checks if the discover pheromone button is pressed.
        /// </summary>
        /// <returns>True if the button is pressed, false otherwise.</returns>
        public bool IsDiscoverPressed() {
            GamePadState cState = GamePad.GetState(_playerIndex);
            KeyboardState kState = Keyboard.GetState();
            return cState.IsButtonDown(_discoverButton) || kState.IsKeyDown(_discoverKey);
        }

        /// <summary>
        /// Checks if the return pheromone button is pressed.
        /// </summary>
        /// <returns>True if the button is pressed, false otherwise.</returns>
        public bool IsReturnPressed() {
            GamePadState cState = GamePad.GetState(_playerIndex);
            KeyboardState kState = Keyboard.GetState();
            return cState.IsButtonDown(_returnButton) || kState.IsKeyDown(_returnKey);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace TinyShopping {

    internal abstract class MenuInput {

        protected PlayerIndex _playerIndex;

        public abstract bool IsSelectPressed();

        public abstract bool IsNextPressed();

        public abstract bool IsPreviousPressed();

        public abstract bool IsBackPressed();

        public abstract bool IsStartedPressed();
    }

    internal class GamePadMenuInput : MenuInput {

        private Buttons _selectButton = Buttons.A;
        private Buttons _nextButton = Buttons.DPadDown;
        private Buttons _previousButton = Buttons.DPadUp;
        private Buttons _backButton = Buttons.B;

        private Buttons _startButton = Buttons.Start;

        public GamePadMenuInput(PlayerIndex playerIndex) {
            _playerIndex = playerIndex;
        }

        public override bool IsSelectPressed() {
            return isButtonPressed(_selectButton);
        }

        public override bool IsBackPressed() {
            return isButtonPressed(_backButton);
        }

        public override bool IsNextPressed() {
            return isButtonPressed(_nextButton);
        }

        public override bool IsPreviousPressed() {
            return isButtonPressed(_previousButton);
        }

        private bool isButtonPressed(Buttons button) {
            GamePadState cState = GamePad.GetState(_playerIndex);
            return cState.IsButtonDown(button);
        }

        public override bool IsStartedPressed()
        {
            GamePadState cState = GamePad.GetState(_playerIndex);
            return cState.IsButtonDown(_startButton);
        }

    }

    internal class KeyboardMenuInput : MenuInput {

        private Keys _nextKey = Keys.Down;

        private Keys _previousKey = Keys.Up;

        private Keys _selectKey = Keys.Enter;

        private Keys _backKey = Keys.Escape;

        private Keys _startKey = Keys.Escape;


        public KeyboardMenuInput(PlayerIndex playerIndex) {
            if (playerIndex < 0 || playerIndex > PlayerIndex.Two) {
                throw new Exception("Invalid player index, maximum 2 players are allowed");
            }
            _playerIndex = playerIndex;
        }

        public override bool IsSelectPressed()
        {
            return IsButtonPressed(_selectKey);
        }
        public override bool IsPreviousPressed()
        {
            return IsButtonPressed(_previousKey);
        }

        public override bool IsNextPressed()
        {
            return IsButtonPressed(_nextKey);
        }

        public override bool IsBackPressed()
        {
            return IsButtonPressed(_backKey);
        }

        public override bool IsStartedPressed()
        {
            KeyboardState kState = Keyboard.GetState();
            return kState.IsKeyDown(_startKey);
        }

        private bool IsButtonPressed(Keys key) {
            KeyboardState kState = Keyboard.GetState();
            return kState.IsKeyDown(key);
        }
    }
}

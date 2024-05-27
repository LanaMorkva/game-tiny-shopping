using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TinyShopping.Game {

    class TutorialMenu: SelectMenu {
        public TutorialMenu(Rectangle menuRegion, Vector2 centerOffset, Vector2 itemSize, Action backAction, Rectangle explanationRegion, SoundPlayer soundPlayer): base(menuRegion, centerOffset, itemSize, backAction, explanationRegion, soundPlayer) {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.IsConnected) {
                _explanations = new List<MenuExplanation> {new("<Right>", "Next", Color.Green)};
            } else {
                _explanations = new List<MenuExplanation> {new("<Enter>", "Next", Color.Green)};
            }
        }

        private bool _continuePressed;

        public override void Update(GameTime gameTime) {
            if (_menuInput.IsStartedPressed()) {
                _continuePressed = true;
            } else if (_continuePressed) {
                _continuePressed = false;
                _soundPlayer.playSoundEffect(_soundEffects[2], 1f);
                _menuItems[_currentSelection].ApplyAction();
            }

        }

    }
}

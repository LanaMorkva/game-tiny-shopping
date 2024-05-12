using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TinyShopping.Game {

    class TutorialMenu: SelectMenu {
        public TutorialMenu(Rectangle menuRegion, Vector2 centerOffset, Vector2 itemSize, Action backAction, Rectangle explanationRegion, List<MenuExplanation> explanations): base(menuRegion, centerOffset, itemSize, backAction, explanationRegion, explanations) {
        }

        private bool _continuePressed;

        public override void Update(GameTime gameTime) {
            if (_menuInput.IsStartedPressed()) {
                _continuePressed = true;
            } else if (_continuePressed) {
                _continuePressed = false;
                _soundEffects[2].Play();
                _menuItems[_currentSelection].ApplyAction();
            }

        }

    }
}

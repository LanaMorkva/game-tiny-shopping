using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping {


    class MenuItemInt: MenuItem {
        private int _state;
        private int _lowerLimit;
        private int _upperLimit;

        private Action _leftAction;
        private Action _rightAction;

        public MenuItemInt(string name, Action leftAction, Action rightAction, int state, int lowerLimit, int upperLimit): base(name, NoAction) {
            _state = state;
            _lowerLimit = lowerLimit;
            _upperLimit = upperLimit;
            _leftAction = leftAction;
            _rightAction = rightAction;
        }

        public override void Draw(SpriteBatch batch, Rectangle itemRect) {
            string text = _name;
            Color color = isSelected ? Color.White : _rectColor;

            text += ": ";
            text += _state > _lowerLimit ? "< " : "";
            text += _state.ToString();
            text += _state < _upperLimit ? " >" : "";

            Vector2 origin = _font.MeasureString(text) / 2;

            batch.Draw(_roundRectTexture, itemRect, color);
            batch.DrawString(_font, text, itemRect.Center.ToVector2(), color, 0, origin, 0.7f, 
                SpriteEffects.None, 0);

        }

        public override void ApplyAction() {
        }

        public override void ApplyActionLeft()
        {
            if (_state > _lowerLimit) {
                _state -= 1;
                _leftAction();
            }
        }

        public override void ApplyActionRight()
        {
            if (_state < _upperLimit) {
                _state += 1;
                _rightAction();
            }
        }

        public static void NoAction() {

        }

    }
}

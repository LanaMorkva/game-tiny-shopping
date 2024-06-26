using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping {

    class MenuItemBool: MenuItem {
        private bool _state;

        public MenuItemBool(string name, Action selectAction, bool state): base(name, selectAction) {
            _state = state;
        }

        public override void Draw(SpriteBatch batch, Rectangle itemRect) {
            string text = _name;
            Color color = isSelected ? Color.White : _rectColor;

            text += ": " + (_state ? "On" : "Off");

            Vector2 origin = _font.MeasureString(text) / 2;

            batch.Draw(_roundRectTexture, itemRect, color);
            batch.DrawString(_font, text, itemRect.Center.ToVector2(), color, 0, origin, 0.7f, 
                SpriteEffects.None, 0);

        }

        public override void ApplyAction() {
            _state = !_state;
            _selectAction();
        }
    }
}

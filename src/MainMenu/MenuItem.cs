using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping.MainMenu {

    internal class MainMenuItem: MenuItem {

        public MainMenuItem(string name, Action nextScene): base(name, nextScene) {
        }

        public override void Draw(SpriteBatch batch, Rectangle itemRect) {
            string text = _name;
            Color color = isSelected ? Color.White : _rectColor;

            batch.Draw(_roundRectTexture, itemRect, color);

            Vector2 origin = _font.MeasureString(text) / 2;
            batch.DrawString(_font, text, itemRect.Center.ToVector2(), _textColor, 0, origin, 0.7f, 
                SpriteEffects.None, 0);
        }
    }
}

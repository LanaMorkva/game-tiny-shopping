using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace TinyShopping.MainMenu {

    internal class MenuItem {

        private string _name;

        private Action _nextScene;

        private Texture2D _roundRectTexture;

        public bool isSelected = false;

        private SpriteFont _font;

        private static readonly Color _rectColor = new Color(241, 164, 132);
        private static readonly Color _textColor = new Color(69, 49, 33);

        public MenuItem(string name, Action nextScene) {
            _name = name;
            _nextScene = nextScene;
        }
        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("General");
            _roundRectTexture = content.Load<Texture2D>("main_menu/rounded_rect");
        }

        public void Draw(SpriteBatch batch, Rectangle itemRect) {
            String text = _name;
            Color color = isSelected ? Color.White : _rectColor;

            batch.Draw(_roundRectTexture, itemRect, color);

            Vector2 origin = _font.MeasureString(text) / 2;
            batch.DrawString(_font, text, itemRect.Center.ToVector2(), _textColor, 0, origin, 0.7f, 
                SpriteEffects.None, 0);
        }

        public Action GetNextScene() {
            return _nextScene;
        }
    }
}

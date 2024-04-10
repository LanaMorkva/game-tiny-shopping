using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace TinyShopping.MainMenu {

    internal class MenuItem {

        private String _name = "Default";

        public Action nextScene;

        public Boolean isSelected = false;

        private SpriteFont _font;

        private Vector2 _basePosition;
        private Vector2 _offset;

        private int _index;

        protected Vector2 BORDER = new(20, 20);

        public MenuItem(String name, Action nextScene, Vector2 offset, Vector2 basePosition, int index) {
            _name = name;
            this.nextScene = nextScene;
            _offset = offset;
            _basePosition = basePosition;
            _index = index;
        }
        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("Arial");
        }

        public void Draw(SpriteBatch batch, GameTime gameTime) {
            String text = _name;
            Color color = Color.White;
            if (isSelected) {
                color = Color.Red;
            }

            Vector2 half = new(0.5f, 0.5f);
            Vector2 box = _font.MeasureString(text) + BORDER * 2;
            Vector2 textHalf = _font.MeasureString(text) * half;
            float yHalf = textHalf.Y;

            Vector2 textPosition = _basePosition + _index * _offset + BORDER;
            batch.FillRectangle(new RectangleF(_basePosition + _offset * _index, box), color);
            batch.DrawString(_font, text, textPosition, Color.Black);

            if (isSelected) {
                batch.DrawLine(textPosition.X - yHalf - BORDER.X, textPosition.Y, textPosition.X - BORDER.X, textPosition.Y + yHalf, Color.Red, 3f);
                batch.DrawLine(textPosition.X - yHalf - BORDER.X, textPosition.Y + 2 * yHalf, textPosition.X - BORDER.X, textPosition.Y + yHalf, Color.Red, 3f);

            }
        }

        public void Update(GameTime gameTime) {
            // needed ?
        }
    }
}

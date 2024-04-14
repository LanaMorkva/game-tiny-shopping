using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace TinyShopping.MainMenu
{

    internal class MenuItem
    {

        private string _name = "Default";

        private Action _nextScene;

        public bool isSelected = false;

        private SpriteFont _font;

        private int _index;

        protected Vector2 _border = new(20, 20);

        protected Vector2 _paddingHeight = new(0, 20);

        private static readonly Color _borderColor = new Color(247, 140, 52);
        private static readonly Color _textColor = new Color(69, 49, 33);

        public MenuItem(string name, Action nextScene, int index, Vector2 border, Vector2 paddingHeight)
        {
            _name = name;
            _nextScene = nextScene;
            _index = index;
            _border = border;
            _paddingHeight = paddingHeight;
        }
        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("General");
        }

        public void Draw(SpriteBatch batch, GameTime gameTime, Vector2 basePosition)
        {
            String text = _name;
            Color color = _borderColor;
            if (isSelected)
            {
                color = Color.White;
            }

            Vector2 box = _font.MeasureString(text) + _border * 2;

            Vector2 height = new(0.0f, box.Y);

            Vector2 textPosition = basePosition + _index * (height + _paddingHeight) + _border;
            batch.FillRectangle(new RectangleF(basePosition + (height + _paddingHeight) * _index, box), color);
            batch.DrawString(_font, text, textPosition, _textColor);


            /*if (isSelected) {
                Vector2 textHalf = _font.MeasureString(text) * new Vector2(0.5f, 0.5f);
                float yHalf = textHalf.Y;
                batch.DrawLine(
                    textPosition.X - yHalf - _border.X - _paddingHeight.Y, 
                    textPosition.Y, 
                    textPosition.X - _border.X - _paddingHeight.Y, 
                    textPosition.Y + yHalf + 5, Color.White, 10f);
                batch.DrawLine(
                    textPosition.X - yHalf - _border.X - _paddingHeight.Y, 
                    textPosition.Y + 2 * yHalf, 
                    textPosition.X - _border.X - _paddingHeight.Y, 
                    textPosition.Y + yHalf - 5, Color.White, 10f);

            }*/
        }

        public Vector2 GetSize()
        {
            return _font.MeasureString(_name) + _border * 2 + _paddingHeight;
        }

        public Action GetNextScene()
        {
            return _nextScene;
        }

        public void Update(GameTime gameTime)
        {
            // not needed
        }
    }
}

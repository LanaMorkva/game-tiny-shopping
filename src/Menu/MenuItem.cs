using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyShopping {

    class MenuItem {

        protected string _name;

        protected Action _selectAction;

        public bool isSelected {get; set;}= false;

        protected SpriteFont _font;

        protected static readonly Color _rectColor = new Color(241, 164, 132);
        protected static readonly Color _textColor = new Color(83, 47, 85);

        private Color _selectColor;

        protected Texture2D _roundRectTexture;

        public MenuItem(string name): this(name, NonAction) {}

        private static void NonAction() {}

        public MenuItem(string name, Action selectAction, Color? selectColor = null) {
            _name = name;
            _selectAction = selectAction;
            _selectColor = selectColor ?? Color.White;
        }
        public virtual void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("fonts/General");
            _roundRectTexture = content.Load<Texture2D>("main_menu/rounded_rect");
        }

        public virtual void UnloadContent(ContentManager content) {
            content.UnloadAsset("fonts/General");
            content.UnloadAsset("main_menu/rounded_rect");
        }

        public virtual void Draw(SpriteBatch batch, Rectangle itemRect) {
            string text = _name;
            Color color = isSelected ? _selectColor : _rectColor;

            batch.Draw(_roundRectTexture, itemRect, color);

            Vector2 origin = _font.MeasureString(text) / 2;
            batch.DrawString(_font, text, itemRect.Center.ToVector2(), color, 0, origin, 0.7f, 
                SpriteEffects.None, 0);
        }

        public virtual void ApplyAction() {
            _selectAction();
        }
        public virtual void ApplyActionLeft() {
            //_selectAction();
        }
        public virtual void ApplyActionRight() {
            //_selectAction();
        }
    }
}

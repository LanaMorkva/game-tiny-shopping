using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyShopping.MainMenu
{

    internal class SelectMenu
    {

        private List<SoundEffect> _soundEffects;

        private int _currentSelection;

        private List<MenuItem> _menuItems;

        private int _nextPressed;

        private int _previousPressed;

        private int _submitPressed;

        private Vector2 _itemBorder;
        private float _itemPadding;

        private Vector2 _basePosition;

        public SelectMenu(Vector2 basePosition, Vector2 border, float padding)
        {
            _soundEffects = new List<SoundEffect>();
            _currentSelection = 0;
            _basePosition = basePosition;

            _menuItems = new List<MenuItem>();
            _itemBorder = border;
            _itemPadding = padding;
        }


        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content)
        {
            _soundEffects.Add(content.Load<SoundEffect>("sounds/glass_knock"));
            foreach (var menuItem in _menuItems)
            {
                menuItem.LoadContent(content);
            }
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            // TODO: Remove unnessecary code duplication by adding better menu input controller
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.IsConnected)
            {
                // Use Gamepad inputs
                if (state.IsButtonDown(Buttons.DPadDown))
                {
                    _nextPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                else if (_nextPressed > 0)
                {
                    nextItem();
                    _nextPressed = 0;
                }
                if (state.IsButtonDown(Buttons.DPadUp))
                {
                    _previousPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                else if (_previousPressed > 0)
                {
                    previousItem();
                    _previousPressed = 0;
                }
                if (state.IsButtonDown(Buttons.A))
                {
                    _submitPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                else if (_submitPressed > 0)
                {
                    _submitPressed = 0;
                    _soundEffects[0].Play();
                    _menuItems[_currentSelection].GetNextScene()();
                }
            }
            else
            {
                // Use keyboard inputs
                KeyboardState kstate = Keyboard.GetState();
                if (kstate.IsKeyDown(Keys.Down))
                {
                    _nextPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                else if (_nextPressed > 0)
                {
                    nextItem();
                    _nextPressed = 0;
                }
                if (kstate.IsKeyDown(Keys.Up))
                {
                    _previousPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                else if (_previousPressed > 0)
                {
                    previousItem();
                    _previousPressed = 0;
                }
                if (kstate.IsKeyDown(Keys.Enter))
                {
                    _submitPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                }
                else if (_submitPressed > 0)
                {
                    _submitPressed = 0;
                    _soundEffects[0].Play();
                    _menuItems[_currentSelection].GetNextScene()();
                }
            }

            setActiveItem();
        }

        private void previousItem()
        {
            _currentSelection -= 1;
            if (_currentSelection < 0)
            {
                _currentSelection = _menuItems.Count - 1;
            }
            _soundEffects[0].Play();
        }

        private void nextItem()
        {
            _currentSelection += 1;
            _currentSelection %= _menuItems.Count;
            _soundEffects[0].Play();
        }

        private void setActiveItem()
        {

            foreach (var it in _menuItems.Select((x, i) => new { Value = x, Index = i }))
            {
                if (it.Index == _currentSelection)
                {
                    it.Value.isSelected = true;
                }
                else
                {
                    it.Value.isSelected = false;
                }
            }
        }

        /// <summary>
        /// Add new item to menu
        /// </summary>
        /// <param name="text">Name of menu item</param>
        /// <param name="nextScene">Function which implements the action that should take place when item is executed</param>
        public void AddItem(string text, Action nextScene)
        {
            _menuItems.Add(new MenuItem(text, nextScene, _menuItems.Count, _itemBorder, new Vector2(0, _itemPadding)));
        }

        /// <summary>
        /// Get total size of menu (all items plus paddings and borders)
        /// </summary>
        /// <returns>total size of menu</returns>
        public Vector2 GetSize()
        {
            Vector2 totalSize = new(0, 0);
            foreach (var item in _menuItems)
            {
                Vector2 itemSize = item.GetSize();
                totalSize.X = Math.Max(itemSize.X, totalSize.X);
                totalSize.Y += itemSize.Y;
            }

            totalSize.Y -= _itemPadding;

            return totalSize;
        }

        /// <summary>
        /// Draws the menu and fps counter.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            batch.FillRectangle(new RectangleF(0, 0, 5000, 5000), new Color(211, 237, 150));
            Vector2 centralizedBasePosition = _basePosition - GetSize() * new Vector2(0.5f, 0.5f);
            foreach (var menuItem in _menuItems)
            {
                menuItem.Draw(batch, gameTime, centralizedBasePosition);
            }
        }
    }
}

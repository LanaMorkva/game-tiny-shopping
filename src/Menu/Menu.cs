using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyShopping {

    class SelectMenu {

        protected List<SoundEffect> _soundEffects;

        protected int _currentSelection;

        protected List<MenuItem> _menuItems;

        protected int _nextPressed;

        protected int _previousPressed;

        protected int _submitPressed;

        protected int _backPressed;

        protected float _itemPadding = 25;
        protected Vector2 _itemSize;

        protected Rectangle _menuRegion;

        protected Vector2 _centerOffset;

        protected Action _backAction;

        public SelectMenu(Rectangle menuRegion, Vector2 itemSize, Action backAction): this(menuRegion, new Vector2(0, 0), itemSize, backAction) {
        }

        public SelectMenu(Rectangle menuRegion, Vector2 centerOffset, Vector2 itemSize, Action backAction) {
            _menuRegion = menuRegion;
            _centerOffset = centerOffset;
            _itemSize = itemSize;

            _currentSelection = 0;
            _soundEffects = new List<SoundEffect>();
            _menuItems = new List<MenuItem>();
            _backAction = backAction;
        }


        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _soundEffects.Add(content.Load<SoundEffect>("sounds/beep-deep"));
            _soundEffects.Add(content.Load<SoundEffect>("sounds/cash_register"));
            _soundEffects.Add(content.Load<SoundEffect>("sounds/beep-extra-deep"));
            foreach (var menuItem in _menuItems) {
                menuItem.LoadContent(content);
            }
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            // TODO: Remove unnessecary code duplication by adding better menu input controller
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.IsConnected) {
                // Use Gamepad inputs
                if (state.IsButtonDown(Buttons.DPadDown)) {
                    _nextPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_nextPressed > 0) {
                    nextItem();
                    _nextPressed = 0;
                }
                if (state.IsButtonDown(Buttons.DPadUp)) {
                    _previousPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_previousPressed > 0) {
                    previousItem();
                    _previousPressed = 0;
                }
                if (state.IsButtonDown(Buttons.A)) {
                    _submitPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_submitPressed > 0) {
                    _submitPressed = 0;
                    _soundEffects[2].Play();
                    _menuItems[_currentSelection].ApplyAction();
                }
                if (state.IsButtonDown(Buttons.B)) {
                    _backPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_backPressed > 0) {
                    _backPressed = 0;
                    _soundEffects[2].Play();
                    _backAction();
                }
            } else {
                // Use keyboard inputs
                KeyboardState kstate = Keyboard.GetState();
                if (kstate.IsKeyDown(Keys.Down)) {
                    _nextPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_nextPressed > 0) {
                    nextItem();
                    _nextPressed = 0;
                }
                if (kstate.IsKeyDown(Keys.Up)) {
                    _previousPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_previousPressed > 0) {
                    previousItem();
                    _previousPressed = 0;
                }
                if (kstate.IsKeyDown(Keys.Enter)) {
                    _submitPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_submitPressed > 0) {
                    _submitPressed = 0;
                    _soundEffects[1].Play();
                    _menuItems[_currentSelection].ApplyAction();
                }
                if (kstate.IsKeyDown(Keys.Back)) {
                    _backPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                } else if (_backPressed > 0) {
                    _backPressed = 0;
                    _soundEffects[1].Play();
                    _backAction();
                }
            }

            setActiveItem();
        }

        private void previousItem() {
            _currentSelection -= 1;
            if (_currentSelection < 0) {
                _currentSelection = _menuItems.Count - 1;
            }
            _soundEffects[0].Play();
        }

        private void nextItem() {
            _currentSelection += 1;
            _currentSelection %= _menuItems.Count;
            _soundEffects[0].Play();
        }

        private void setActiveItem() {
            foreach (var it in _menuItems.Select((x, i) => new { Value = x, Index = i })) {
                it.Value.isSelected = it.Index == _currentSelection;
            }
        }

        /// <summary>
        /// Add new item to menu
        /// </summary>
        /// <param name="text">Name of menu item</param>
        /// <param name="nextScene">Function which implements the action that should take place when item is executed</param>
        public void AddItem(MenuItem item) {
            _menuItems.Add(item);
            setActiveItem();
        }


        /// <summary>
        /// Get total size of menu (all items plus paddings and borders)
        /// </summary>
        /// <returns>total size of menu</returns>
        public Vector2 GetSize() {
            Vector2 totalSize = new(0, 0);
            foreach (var item in _menuItems) {
                totalSize.X = Math.Max(_itemSize.X, totalSize.X);
                totalSize.Y += _itemSize.Y + _itemPadding;
            }

            totalSize.Y -= _itemPadding;

            return totalSize;
        }

        /// <summary>
        /// Draws the menu and fps counter.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch) {
            Vector2 menuLocation = _menuRegion.Center.ToVector2() - GetSize() / 2;
            //menuLocation.Y -= _menuRegion.Y / 3;
            menuLocation += _centerOffset;
            foreach (var menuItem in _menuItems) {
                var itemRect = new Rectangle(menuLocation.ToPoint(), _itemSize.ToPoint());
                menuItem.Draw(batch, itemRect);
                menuLocation += new Vector2(0, _itemSize.Y + _itemPadding);
            }
        }
    }
}

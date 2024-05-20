using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyShopping {

    struct MenuExplanation {
        public string button;
        public string text;
        public Color buttonColor;

        public MenuExplanation(string Button, string Text, Color ButtonColor) {
            button = Button;
            text = Text;
            buttonColor = ButtonColor;
        }
    }

    class SelectMenu {

        protected List<SoundEffect> _soundEffects;

        protected SpriteFont _font;

        protected int _currentSelection;

        protected List<MenuItem> _menuItems;

        protected int _nextPressed;
        protected int _lastNext;

        protected int _previousPressed;
        protected int _lastPrevious;

        protected int _submitPressed;

        protected int _backPressed;
        protected int _leftPressed;
        protected int _lastLeft;
        protected int _rightPressed;
        protected int _lastRight;

        protected float _itemPadding = 25;
        protected Vector2 _itemSize;

        protected Rectangle _menuRegion;

        protected Vector2 _centerOffset;

        protected Action _backAction;

        protected MenuInput _menuInput;

        protected Rectangle _explanationRegion;

        protected List<MenuExplanation> _explanations;

        public SelectMenu(Rectangle menuRegion, Vector2 itemSize, Action backAction, Rectangle explanationRegion) : this(menuRegion, new Vector2(0, 0), itemSize, backAction, explanationRegion, CreateDefaultExplanations()) {
        }

        public SelectMenu(Rectangle menuRegion, Vector2 itemSize, Action backAction, Rectangle explanationRegion, List<MenuExplanation> explanations) : this(menuRegion, new Vector2(0, 0), itemSize, backAction, explanationRegion, explanations) {
        }

        public SelectMenu(Rectangle menuRegion, Vector2 centerOffset, Vector2 itemSize, Action backAction, Rectangle explanationRegion) : this(menuRegion, centerOffset, itemSize, backAction, explanationRegion, CreateDefaultExplanations()) {
        }

        public SelectMenu(Rectangle menuRegion, Vector2 centerOffset, Vector2 itemSize, Action backAction, Rectangle explanationRegion, List<MenuExplanation> explanations) {
            _menuRegion = menuRegion;
            _centerOffset = centerOffset;
            _itemSize = itemSize;

            _currentSelection = 0;
            _soundEffects = new List<SoundEffect>();
            _menuItems = new List<MenuItem>();
            _backAction = backAction;
            _menuInput = CreateMenuInput(PlayerIndex.One);

            _explanationRegion = explanationRegion;
            _explanations = explanations;
        }

        private static List<MenuExplanation> CreateDefaultExplanations() {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.IsConnected) {
                return new List<MenuExplanation> { new("<A>", "Select", Color.Green) };
            } else {
                return new List<MenuExplanation> { new("<Enter>", "Select", Color.Green) };
            }
        }


        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("fonts/General");
            _soundEffects.Add(content.Load<SoundEffect>("sounds/beep-deep"));
            _soundEffects.Add(content.Load<SoundEffect>("sounds/cash_register"));
            _soundEffects.Add(content.Load<SoundEffect>("sounds/beep-extra-deep"));
            foreach (var menuItem in _menuItems) {
                menuItem.LoadContent(content);
            }
        }

        public void UnloadContent(ContentManager content) {
            content.UnloadAsset("fonts/General");
            content.UnloadAsset("sounds/beep-deep");
            content.UnloadAsset("sounds/cash_register");
            content.UnloadAsset("sounds/beep-extra-deep");
            foreach (var menuItem in _menuItems) {
                menuItem.UnloadContent(content);
            }
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public virtual void Update(GameTime gameTime) {
            if (_menuInput.IsNextPressed()) {
                _nextPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                if (_lastNext < _nextPressed) {
                    _soundEffects[2].Play();
                    nextItem();
                    if (_lastNext == 0) {
                        _lastNext += 500;
                    } else {
                        _lastNext += 100;
                    }
                }
            } else if (_nextPressed > 0) {
                //nextItem();
                _lastNext = 0;
                _nextPressed = 0;
            }
            if (_menuInput.IsPreviousPressed()) {
                _previousPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                if (_lastPrevious < _previousPressed) {
                    _soundEffects[2].Play();
                    previousItem();
                    if (_lastPrevious == 0) {
                        _lastPrevious += 500;
                    } else {
                        _lastPrevious += 100;
                    }
                }
            } else if (_previousPressed > 0) {
                //previousItem();
                _lastPrevious = 0;
                _previousPressed = 0;
            }
            if (_menuInput.IsSelectPressed()) {
                _submitPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
            } else if (_submitPressed > 0) {
                _submitPressed = 0;
                _soundEffects[2].Play();
                _menuItems[_currentSelection].ApplyAction();
            }
            if (_menuInput.IsLeftPressed()) {
                _leftPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                if (_lastLeft < _leftPressed) {
                    _soundEffects[2].Play();
                    _menuItems[_currentSelection].ApplyActionLeft();
                    if (_lastLeft == 0) {
                        _lastLeft += 500;
                    } else {
                        _lastLeft += 100;
                    }
                }
            } else if (_leftPressed > 0) {
                _leftPressed = 0;
                _lastLeft = 0;
                //_soundEffects[2].Play();
            }
            if (_menuInput.IsRightPressed()) {
                _rightPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
                if (_lastRight < _rightPressed) {
                    _menuItems[_currentSelection].ApplyActionRight();
                    _soundEffects[2].Play();
                    if (_lastRight == 0) {
                        _lastRight += 500;
                    } else {
                        _lastRight += 100;
                    }
                }
            } else if (_rightPressed > 0) {
                _rightPressed = 0;
                _lastRight = 0;
                //_soundEffects[2].Play();
                //_menuItems[_currentSelection].ApplyActionRight();
            }
            if (_menuInput.IsBackPressed()) {
                _backPressed += (int)Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
            } else if (_backPressed > 0) {
                _backPressed = 0;
                _soundEffects[2].Play();
                _backAction();
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

            //batch.DrawRectangle(_explanationRegion, Color.Red);
            int i = 0;
            foreach (var explanation in _explanations) {
                float scale = 0.5f;
                Vector2 buttonSize = _font.MeasureString(explanation.button) * scale;
                float x = _explanationRegion.X;
                float y = i * (_explanationRegion.Height / _explanations.Count) + _explanationRegion.Y;
                Vector2 origin = new Vector2(_explanationRegion.X, _explanationRegion.Y);
                batch.DrawString(_font, explanation.text, new Vector2(x + buttonSize.X + 10, y), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                batch.DrawString(_font, explanation.button, new Vector2(x, y), explanation.buttonColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                i++;
            }
        }

        private MenuInput CreateMenuInput(PlayerIndex playerIndex) {
            GamePadState state = GamePad.GetState(playerIndex);
            if (state.IsConnected) {
                return new GamePadMenuInput(playerIndex);
            }
            return new KeyboardMenuInput(playerIndex);
        }

        public void ResetActiveItem() {
            _currentSelection = 0;
        }
    }
}

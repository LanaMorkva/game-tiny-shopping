using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyShopping.MainMenu {

    internal class SelectMenu {

        private SpriteFont _font;

        private GraphicsDevice _device;

        private Texture2D _backgroundTexture;

        private List<SoundEffect> _soundEffects;

        private Scene _scene;

        private int _currentSelection;

        private List<MenuItem> menuItems;

        private int _nextPressed;

        private int _previousPressed;
        
        private int _submitPressed;

        public SelectMenu(GraphicsDevice device, Scene scene) {
            _device = device;
            _scene = scene;
            _soundEffects = new List<SoundEffect>();
            _currentSelection = 0;
            
            Vector2 basePosition = new(_scene.Width * 0.382f - 300, _scene.Height / 2.0f - 300);
            Console.WriteLine(_scene.Width);
            Console.WriteLine(basePosition);
            Vector2 offset = new(0, 100);
            menuItems = new List<MenuItem>
            {
                new MenuItem("New Game", _scene.StartGame, offset, basePosition, 0),
                new MenuItem("How to play", _scene.NotImplementedScene, offset, basePosition, 1),
                new MenuItem("Settings", _scene.NotImplementedScene, offset, basePosition, 2),
                new MenuItem("Quit", _scene.ExitGame, offset, basePosition, 3),
            };
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _backgroundTexture = content.Load<Texture2D>("teaser");
            _font = content.Load<SpriteFont>("Arial");
            _soundEffects.Add(content.Load<SoundEffect>("sounds/glass_knock"));
            foreach (var menuItem in menuItems)
            {
                menuItem.LoadContent(content); 
            }
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
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
                    _soundEffects[0].Play();
                    menuItems[_currentSelection].nextScene();
                }
            } else {
                // Use keyboard inputs
                KeyboardState kstate = Keyboard.GetState(PlayerIndex.One);
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
                    _soundEffects[0].Play();
                    menuItems[_currentSelection].nextScene();
                }
            }

            // Check if selection button was pushed to go to respective scene

            // Check if up or down was pushed to update selected item


            setActiveItem();
        }

        private void previousItem() {
            _currentSelection -= 1;
            if (_currentSelection < 0) {
                _currentSelection = menuItems.Count - 1;
            }
            _soundEffects[0].Play();
        }

        private void nextItem() {
            _currentSelection += 1;
            _currentSelection %= menuItems.Count;
            _soundEffects[0].Play();
        }

        private void setActiveItem() {

            foreach (var it in menuItems.Select((x, i) => new { Value = x, Index = i }) )
            {
                if (it.Index == _currentSelection) {
                    it.Value.isSelected = true;
                } else {
                    it.Value.isSelected = false;
                }
            }
        }

        /// <summary>
        /// Draws the statistics and fps counter.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            Vector2 backgroundCenter = new((int)(_scene.Width / 1.618), (int)(_scene.Height/2.0));

            batch.FillRectangle(new RectangleF(0,0,5000,5000), new Color(211, 237, 150));
            foreach (var menuItem in menuItems)
            {
                menuItem.Draw(batch, gameTime);
            }

            Vector2 backgroundSize = new(500, 500);
            Vector2 origin = backgroundCenter - backgroundSize;
            batch.Draw(_backgroundTexture, new Rectangle((int)origin.X,(int) origin.Y,2*(int)backgroundSize.X,2*(int) backgroundSize.Y), Color.White);

#if DEBUG
            int fps = (int) Math.Round((1000 / gameTime.ElapsedGameTime.TotalMilliseconds));
            batch.DrawString(_font, "FPS: " + fps.ToString(), new Vector2(0, 0), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
#endif
        }


    }
}

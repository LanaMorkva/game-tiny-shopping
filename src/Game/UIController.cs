using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace TinyShopping.Game {

    internal class UIController {

        private static readonly Color _borderColor = new Color(247, 140, 52);
        private static readonly Color _textColor = new Color(69, 49, 33);

        private SpriteFont _font;
        private SpriteFont _fontBig;
        private SpriteFont _fontGeneral;

        private Texture2D _appleTexture;

        private Texture2D _circleTexture;

        private Texture2D _roundRectTexture;

        private Texture2D _antsCharachterTexture;

        private Texture2D _termiteCharachterTexture;

        private SplitScreenHandler _handler;

        private List<SoundEffect> _soundEffects;

        private Scene _scene;

        private double _runtimeMs;

        private int _winner;

        private MenuInput _playerOne;

        private bool _selectPressed;

        public UIController(GraphicsDevice device, SplitScreenHandler handler, Scene scene) {
            _handler = handler;
            _runtimeMs = 0;
            _scene = scene;
            _soundEffects = new List<SoundEffect>();
            _playerOne = CreateMenuInput(PlayerIndex.One);
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("fonts/Fun");
            _fontBig = content.Load<SpriteFont>("fonts/FunBig");
            _fontGeneral = content.Load<SpriteFont>("fonts/General");

            _appleTexture = content.Load<Texture2D>("stats/apple");
            _circleTexture = content.Load<Texture2D>("stats/circle");
            _antsCharachterTexture = content.Load<Texture2D>("stats/Ant_Icon");
            _termiteCharachterTexture = content.Load<Texture2D>("stats/Termite_Icon");
            _roundRectTexture = content.Load<Texture2D>("stats/rounded_rectangle");

            _soundEffects.Add(content.Load<SoundEffect>("sounds/countdown_3_seconds"));
            _soundEffects.Add(content.Load<SoundEffect>("sounds/final_whistle"));
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            bool isOverBefore = _scene.IsOver;
            if (!_scene.IsStarted) {
                if (_runtimeMs == 0) {
                    _soundEffects[0].Play();
                }
                _runtimeMs += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_runtimeMs >= 4000) {
                    _runtimeMs = 0.0;
                    _scene.IsStarted = true;
                }
            } else {
                if (!_scene.IsOver) {
                    _runtimeMs += gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                if (_runtimeMs > Constants.TIME_LIMIT_S * 1000) {
                    _scene.IsOver = true;
                    if (_handler.GetNumberOfFruits(0) > _handler.GetNumberOfFruits(1)) {
                        _winner = 1;
                    }
                    if (_handler.GetNumberOfFruits(0) < _handler.GetNumberOfFruits(1)) {
                        _winner = 2;
                    }
                }
                if (_handler.GetNumberOfAnts(0) == 0) {
                    _scene.IsOver = true;
                    _winner = 1;
                }
                if (_handler.GetNumberOfAnts(1) == 0) {
                    _scene.IsOver = true;
                    _winner = 2;
                }

            }

            if (!isOverBefore && _scene.IsOver) {
                _soundEffects[1].Play();
            }

            if (_scene.IsOver && _playerOne.IsSelectPressed()) {
                _selectPressed = true;
            }
            else if (_selectPressed) {
                _selectPressed = false;
                _scene.LoadMainMenu();
            }

        }

        private MenuInput CreateMenuInput(PlayerIndex playerIndex) {
            GamePadState state = GamePad.GetState(playerIndex);
            if (state.IsConnected) {
                return new GamePadMenuInput(playerIndex);
            }
            return new KeyboardMenuInput(playerIndex);
        }

        /// <summary>
        /// Draws the statistics and fps counter.
        /// </summary>
        /// <param name="batch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            DrawBorder(batch);
            DrawStatistics(batch);
            DrawRemainingTime(batch);
            if (!_scene.IsStarted) {
                DrawCountdown(batch);
            }
            if (_scene.IsOver) {
                DrawWinMessage(batch);
                DrawReturnMessage(batch);
            }
#if DEBUG
            int fps = (int) Math.Round((1000 / gameTime.ElapsedGameTime.TotalMilliseconds));
            DrawString(batch, "FPS: " + fps.ToString(), new Vector2(80, 20));
#endif
        }

        /// <summary>
        /// Draws screen separation
        /// </summary>
        /// <param name="batch">The batch to use.</param>
        private void DrawBorder(SpriteBatch batch) {
            var splitBorder = new Rectangle(_scene.Width / 2 - 5, 0, 10, _scene.Height);
            var timeBorder = new Rectangle(_scene.Width / 2 - 90, 0, 180, 40);

            var splitBorderBig = splitBorder;
            var timeBorderBig = timeBorder;
            splitBorderBig.Inflate(3, 3);
            timeBorderBig.Inflate(3, 3);

            batch.FillRectangle(splitBorderBig, _textColor);
            batch.Draw(_roundRectTexture, timeBorderBig, _textColor);

            batch.FillRectangle(splitBorder, _borderColor);
            batch.Draw(_roundRectTexture, timeBorder, _borderColor);
        }

        /// <summary>
        /// Writes the game over and win text to the screen.
        /// </summary>
        /// <param name="batch">The batch to use.</param>
        private void DrawWinMessage(SpriteBatch batch) {
            string text = "IT'S A DRAW!";
            if (_winner !=  0) {
                text = "PLAYER " + _winner + " WINS THIS ROUND!";
            }
            DrawDimScreen(batch);

            var pos = new Vector2(_scene.Width / 2, _scene.Height / 2);
            Vector2 origin = _fontGeneral.MeasureString(text) / 2;
            batch.DrawString(_fontGeneral, text, pos - new Vector2(3, 3), Color.Black, 0, origin, 1.0f, SpriteEffects.None, 0);
            batch.DrawString(_fontGeneral, text, pos, _textColor, 0, origin, 1.0f, SpriteEffects.None, 0);
        }

        private void DrawReturnMessage(SpriteBatch batch) {
            string text = "Press ";
            if (GamePad.GetState(PlayerIndex.One).IsConnected) {
                text += "<A> ";
            } else {
                text += "<Enter> ";
            }
            text += "to return to main menu";

            Vector2 origin = _fontGeneral.MeasureString(text) / 2;
            var pos = new Vector2(_scene.Width / 2, _scene.Height - origin.Y - 10);
            batch.DrawString(_fontGeneral, text, pos, _textColor, 0, origin, 0.5f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws the time countdown.
        /// </summary>
        /// <param name="batch"></param>
        private void DrawRemainingTime(SpriteBatch batch) {
            int offsetTop = 20;
            int secs;
            if (_scene.IsStarted) {
                secs = Constants.TIME_LIMIT_S - (int) (_runtimeMs / 1000);
            } else {
                secs = Constants.TIME_LIMIT_S;
            }
            int mins = secs / 60;
            secs %= 60;
            string minStr = (mins < 10 ? "0" : "") + mins;
            string secStr = (secs < 10 ? "0" : "") + secs;
            string time = minStr + ":" + secStr;
            if (secs < 0) {
                time = "00:00";
            }
            DrawString(batch, time, new Vector2(_scene.Width / 2, offsetTop));
        }

        public float GetRemainingTime() {
            if (!_scene.IsStarted) {
                return Constants.TIME_LIMIT_S;
            } else if (_scene.IsOver) {
                return 0f;
            } else {
                return (float) (Constants.TIME_LIMIT_S - (_runtimeMs / 1000f));
            }
        }

        /// <summary>
        /// Draw Countdown at the start of the game
        /// </summary>
        /// <param name="batch">The sprite batch to write to.</param>
        private void DrawCountdown(SpriteBatch batch) {
            int secs = 3 - (int) (_runtimeMs / 1000);
            string secStr = "" + secs;
            if (secs == 0) {
                secStr = "Go!";
            }
            DrawDimScreen(batch);
            DrawBoldString(batch, secStr, new Vector2(_scene.Width / 2, _scene.Height / 2), 1.5f, 0.06f);
        }

        private void DrawDimScreen(SpriteBatch batch) {
            batch.FillRectangle(new Rectangle(0, 0, _scene.Width, _scene.Height), new Color(122, 119, 110, 120));
        }
        private void DrawString(SpriteBatch batch, String text, Vector2 position) {
            Vector2 origin = _font.MeasureString(text) / 2;
            batch.DrawString(_font, text, position, _textColor, 0, origin, 0.95f, SpriteEffects.None, 0);
        }
        private void DrawBoldString(SpriteBatch batch, String text, Vector2 position, float scale, float border = .005f) {
            Vector2 origin = _fontBig.MeasureString(text) / 2;

            batch.DrawString(_fontBig, text, position, Color.Black, 0, origin, scale + border, SpriteEffects.None, 0);
            batch.DrawString(_fontBig, text, position, _textColor, 0, origin, scale, SpriteEffects.None, 0);
        }

        private void DrawOutlinedTexture(SpriteBatch batch, Texture2D texture, Rectangle rect, Color color, int border = 5, bool flipped = false) {
            var rectBig = rect;
            rectBig.Inflate(border, border);

            if (!flipped) {
                batch.Draw(texture, rectBig, _textColor);
                batch.Draw(texture, rect, color);
            } else {
                batch.Draw(texture, rectBig, null, _textColor, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                batch.Draw(texture, rect, null, color, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
            }
        }

        /// <summary>
        /// Draws the two players fruit and insect statistics.
        /// </summary>
        /// <param name="batch">The sprite batch to write to.</param>
        private void DrawStatistics(SpriteBatch batch) {
            DrawOutlinedTexture(batch, _antsCharachterTexture, new Rectangle(30, 30, 160, 160), Color.White);
            DrawOutlinedTexture(batch, _termiteCharachterTexture, new Rectangle(_scene.Width - 180, 30, 160, 160), Color.White);

            var appleRect1 = new Rectangle(145, 90, 80, 80);
            var antsRect1 = new Rectangle(100, 130, 70, 70);

            var appleRect2 = new Rectangle(_scene.Width - 225, 90, 80, 80);
            var antsRect2 = new Rectangle(_scene.Width - 170, 130, 70, 70);

            DrawOutlinedTexture(batch, _appleTexture, appleRect1, Color.White, 3);
            DrawOutlinedTexture(batch, _circleTexture, antsRect1, new Color(240, 137, 55), 2);

            DrawOutlinedTexture(batch, _appleTexture, appleRect2, Color.White, 3, true);
            DrawOutlinedTexture(batch, _circleTexture, antsRect2, new Color(240, 137, 55), 2);

            String AntsNum1 = "x" + _handler.GetNumberOfAnts(0);
            DrawString(batch, AntsNum1, antsRect1.Center.ToVector2());

            String FruitsNum1 = "x" + _handler.GetNumberOfFruits(0);
            DrawString(batch, FruitsNum1, appleRect1.Center.ToVector2() - new Vector2(4, -6));

            String AntsNum2 = "x" + _handler.GetNumberOfAnts(1);
            DrawString(batch, AntsNum2, antsRect2.Center.ToVector2());

            String FruitsNum2 = "x" + _handler.GetNumberOfFruits(1);
            DrawString(batch, FruitsNum2, appleRect2.Center.ToVector2() - new Vector2(-4, -6));

        }
    }
}

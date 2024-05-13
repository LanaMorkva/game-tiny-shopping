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

        private static readonly Color _borderColor = new Color(255, 157, 118);
        private static readonly Color _textColor = new Color(83, 47, 85);
        private static readonly float _fontScale = 0.4f;

        private SpriteFont _font;
        private SpriteFont _fontBig;
        private SpriteFont _fontGeneral;

        private Texture2D _appleTexture;

        private Texture2D _circleTexture;

        private Texture2D _roundRectTexture;

        private Texture2D _antsCharachterTexture;

        private Texture2D _termiteCharachterTexture;

        private Texture2D _gamepadButtons;

        private Texture2D _keyboardButtons;

        private SplitScreenHandler _handler;

        private List<SoundEffect> _soundEffects;

        private Scene _scene;

        private double _runtimeMs;

        private double _countdownMs;

        private double _afterGameMs;

        private int _winner;

        private MenuInput _playerOne;

        private bool _selectPressed;

        private UIInsectController _insectController;

        public UIController(GraphicsDevice device, SplitScreenHandler handler, Scene scene) {
            _handler = handler;
            _runtimeMs = 0;
            _scene = scene;
            _soundEffects = new List<SoundEffect>();
            _playerOne = CreateMenuInput(PlayerIndex.One);
            _insectController = new UIInsectController(handler);
            _afterGameMs = 3000;
        }

        /// <summary>
        /// Loads the necessary data.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("fonts/General");
            _fontBig = content.Load<SpriteFont>("fonts/General"); //FunBig
            _fontGeneral = content.Load<SpriteFont>("fonts/General");

            _appleTexture = content.Load<Texture2D>("stats/apple");
            _circleTexture = content.Load<Texture2D>("stats/circle");
            _antsCharachterTexture = content.Load<Texture2D>("stats/Ant_Icon");
            _termiteCharachterTexture = content.Load<Texture2D>("stats/Termite_Icon");
            _roundRectTexture = content.Load<Texture2D>("stats/rounded_rectangle");

            _soundEffects.Add(content.Load<SoundEffect>("sounds/countdown_3_seconds"));
            _soundEffects.Add(content.Load<SoundEffect>("sounds/final_whistle"));

            _gamepadButtons = content.Load<Texture2D>("stats/controller");
            _keyboardButtons = content.Load<Texture2D>("stats/numbers");

            _insectController.LoadContent(content);
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            if (_scene.gameState == GameState.StartCountdown) {
                if (_countdownMs == 0) {
                    _soundEffects[0].Play();
                }
                _countdownMs += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_countdownMs >= 4000) {
                    _countdownMs = 0.0;
                    _scene.gameState = GameState.Playing;
                }
            } else if (_scene.gameState == GameState.Playing) {
                _runtimeMs += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_runtimeMs > Constants.TIME_LIMIT_S * 1000) {
                    _scene.gameState = GameState.Ended;
                    if (_handler.GetNumberOfFruits(0) > _handler.GetNumberOfFruits(1)) {
                        _winner = 1;
                    }
                    if (_handler.GetNumberOfFruits(0) < _handler.GetNumberOfFruits(1)) {
                        _winner = 2;
                    }
                }
                if (_handler.GetNumberOfAnts(0) == 0 || _handler.GetSpawnHealth(0) <= 0) {
                    _scene.gameState = GameState.Ended;
                    _winner = 2;
                }
                if (_handler.GetNumberOfAnts(1) == 0 || _handler.GetSpawnHealth(1) <= 0) {
                    _scene.gameState = GameState.Ended;
                    _winner = 1;
                }

                // Play end of game sound
                if (_scene.gameState == GameState.Ended) {
                    _soundEffects[1].Play();
                }

            } else if (_scene.gameState == GameState.Ended) {
                _afterGameMs -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_afterGameMs <= 0 && _playerOne.IsSelectPressed()) {
                    _selectPressed = true;
                } else if (_selectPressed) {
                    _selectPressed = false;
                    _scene.LoadMainMenu();
                }
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
        public void Draw(SpriteBatch batch, GameTime gameTime, Vector2 player1_pos, Vector2 player2_pos, bool player1_keyboard, bool player2_keyboard) {
            DrawBorder(batch);
            DrawStatistics(batch);
            DrawRemainingTime(batch);
            _insectController.Draw(batch, gameTime);
            if (_runtimeMs > 10000) {
                DrawControls(batch);
            } 
            if (_scene.gameState == GameState.StartCountdown) {
                DrawCountdown(batch);
            } else if (_scene.gameState == GameState.Playing) {
                if (_runtimeMs < 10000) {
                    var buttonColor = new Color(122, 119, 110, 200);
                    DrawCursorExplanations(batch, player1_pos, buttonColor, PlayerIndex.One, true, player1_keyboard);
                    DrawCursorExplanations(batch, player2_pos, buttonColor, PlayerIndex.Two, true, player2_keyboard);
                } 
            } else if (_scene.gameState == GameState.Ended) {
                DrawWinMessage(batch);
                if (_afterGameMs <= 0) {
                    DrawReturnMessage(batch);
                }
            }
#if DEBUG
            int fps = (int) Math.Round((1000 / gameTime.ElapsedGameTime.TotalMilliseconds));
            DrawString(batch, "FPS: " + fps.ToString(), new Vector2(80, 20), _fontScale);
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
            //batch.Draw(_roundRectTexture, timeBorderBig, _textColor);

            batch.FillRectangle(splitBorder, _borderColor);
            batch.Draw(_roundRectTexture, timeBorder, Color.White);
        }

        /// <summary>
        /// Writes the game over and win text to the screen.
        /// </summary>
        /// <param name="batch">The batch to use.</param>
        private void DrawWinMessage(SpriteBatch batch) {
            DrawDimScreen(batch);

            var partHeight = _scene.Height / 3.5f;
            var imagePos = new Vector2(_scene.Width / 2, partHeight);
            var textureSize = new Vector2(_antsCharachterTexture.Width, _antsCharachterTexture.Height) * 1.2f;

            string text = "IT'S A DRAW!";
            if (_winner !=  0) {
                text = _winner == 1 ? "ANTS WIN THIS ROUND!" : "TERMITES WIN THIS ROUND!";
                var texture = _winner == 1 ? _antsCharachterTexture : _termiteCharachterTexture;
                batch.Draw(texture, new RectangleF(imagePos - textureSize / 2, textureSize).ToRectangle(), Color.White);
            } else {
                int gap = 75;
                var antsCharachterPos = new Vector2(imagePos.X - textureSize.X - gap, imagePos.Y - textureSize.Y / 2);
                var termiteCharachterPos = new Vector2(imagePos.X + gap, imagePos.Y - textureSize.Y / 2);
                batch.Draw(_antsCharachterTexture, new RectangleF(antsCharachterPos, textureSize).ToRectangle(), Color.White);
                batch.Draw(_termiteCharachterTexture, new RectangleF(termiteCharachterPos, textureSize).ToRectangle(), Color.White);
            }

            var textPos = imagePos + new Vector2(0, textureSize.Y);
            Vector2 origin = _fontGeneral.MeasureString(text) / 2;
            batch.DrawString(_fontGeneral, text, textPos - new Vector2(3, 3), Color.Black, 0, origin, 1.0f, SpriteEffects.None, 0);
            batch.DrawString(_fontGeneral, text, textPos, _textColor, 0, origin, 1.0f, SpriteEffects.None, 0);
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
            batch.DrawString(_fontGeneral, text, pos, _textColor, 0, origin, 0.4f, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws the time countdown.
        /// </summary>
        /// <param name="batch"></param>
        private void DrawRemainingTime(SpriteBatch batch) {
            int offsetTop = 20;
            int secs;
            if (_scene.gameState == GameState.Playing || _scene.gameState == GameState.Paused) {
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
            DrawString(batch, time, new Vector2(_scene.Width / 2, offsetTop), _fontScale);
        }

        public float GetRemainingTime() {
            if (_scene.gameState == GameState.StartCountdown) {
                return Constants.TIME_LIMIT_S;
            } else if (_scene.gameState == GameState.Ended) {
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
            int secs = 3 - (int) (_countdownMs / 1000);
            string secStr = "" + secs;
            if (secs == 0) {
                secStr = "Go!";
            }
            DrawDimScreen(batch);
            DrawBoldString(batch, secStr, new Vector2(_scene.Width / 2, _scene.Height / 2), 1.5f, 0.06f);
        }

        private void DrawDimScreen(SpriteBatch batch) {
            batch.FillRectangle(new Rectangle(0, 0, _scene.Width, _scene.Height), new Color(122, 119, 110, 160));
        }
        private void DrawString(SpriteBatch batch, String text, Vector2 position, float scale) {
            Vector2 origin = _font.MeasureString(text) / 2;
            batch.DrawString(_font, text, position, _textColor, 0, origin, scale, SpriteEffects.None, 0); // scale used to be 0.95f
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
                //batch.Draw(texture, rectBig, _textColor);
                batch.Draw(texture, rect, color);
            } else {
                //batch.Draw(texture, rectBig, null, _textColor, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
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
            DrawOutlinedTexture(batch, _circleTexture, antsRect1, Color.White, 2);

            DrawOutlinedTexture(batch, _appleTexture, appleRect2, Color.White, 3, true);
            DrawOutlinedTexture(batch, _circleTexture, antsRect2, Color.White, 2);

            String AntsNum1 = "x" + _handler.GetNumberOfAnts(0);
            DrawString(batch, AntsNum1, antsRect1.Center.ToVector2(), _fontScale);

            String FruitsNum1 = "x" + _handler.GetNumberOfFruits(0);
            DrawString(batch, FruitsNum1, appleRect1.Center.ToVector2() - new Vector2(-2, 2), _fontScale);

            String AntsNum2 = "x" + _handler.GetNumberOfAnts(1);
            DrawString(batch, AntsNum2, antsRect2.Center.ToVector2(), _fontScale);

            String FruitsNum2 = "x" + _handler.GetNumberOfFruits(1);
            DrawString(batch, FruitsNum2, appleRect2.Center.ToVector2() - new Vector2(-1, -1), _fontScale);
        }

        private void DrawControls(SpriteBatch batch) {
            int gap = 7;
            int borderGap = 20;
            int bottomOffset = 15;
            int textOffset = 5;
            var textureSize = new Vector2(_scene.Height / 35, _scene.Height / 35);
            
            for (int i = 0; i < _handler.Controls1.Count; i++){
                var control = _handler.Controls1[i];
                var heightOffset = (_handler.Controls1.Count - i) * (textureSize.Y + gap) + bottomOffset;
                var controlPos = new Vector2(_scene.Width / 2 - borderGap - textureSize.X, _scene.Height - heightOffset);
                batch.Draw(control.Texture, new RectangleF(controlPos, textureSize).ToRectangle(), 
                            control.SourceRect, new Color(122, 119, 110, 160));
                
                var text = control.Description;
                Vector2 stringSize = _font.MeasureString(text);
                Vector2 origin = new Vector2(stringSize.X, stringSize.Y / 2);
                var textPos = controlPos + new Vector2(-textOffset, textureSize.Y / 2);
                batch.DrawString(_font, text, textPos, new Color(71, 71, 68, 200), 0, origin, 0.15f, SpriteEffects.None, 0); 
            }

            for (int i = 0; i < _handler.Controls2.Count; i++){
                var control = _handler.Controls2[i];
                var heightOffset = (_handler.Controls2.Count - i) * (textureSize.Y + gap) + bottomOffset;
                var controlPos = new Vector2(_scene.Width / 2 + borderGap, _scene.Height - heightOffset);
                batch.Draw(control.Texture, new RectangleF(controlPos, textureSize).ToRectangle(), 
                            control.SourceRect, new Color(122, 119, 110, 160));
                
                var text = control.Description;
                Vector2 stringSize = _font.MeasureString(text);
                Vector2 origin = new Vector2(0, stringSize.Y / 2);
                var textPos = controlPos + new Vector2(textOffset + textureSize.X, textureSize.Y / 2);
                batch.DrawString(_font, text, textPos, new Color(71, 71, 68, 200), 0, origin, 0.15f, SpriteEffects.None, 0); 
            }
        }

        private void DrawCursorExplanations(SpriteBatch batch, Vector2 player_position, Color transparency, PlayerIndex index, bool showText, bool isPlayerOnKeyboard) {
            string battleText = "Fight";
            string discoverText = "Discover";
            string returnText = "Return home";
            string newAntText = "Buy new insect";

            float distance = _scene.Height / 10;

            float scale = 0.18f;
            Vector2 battleTextSize = _font.MeasureString(battleText) * scale;
            battleTextSize = new Vector2(distance, -(battleTextSize.Y/2f));
            Vector2 discoverTextSize = _font.MeasureString(discoverText) * scale;
            discoverTextSize = new Vector2(-discoverTextSize.X/2f, (-discoverTextSize.Y/2f)+distance);
            Vector2 returnTextSize = _font.MeasureString(returnText) * scale;
            returnTextSize = new Vector2(-returnTextSize.X-distance, -(returnTextSize.Y/2f));
            Vector2 newAntTextSize = _font.MeasureString(newAntText) * scale;
            newAntTextSize = new Vector2(-newAntTextSize.X/2f, (-newAntTextSize.Y/2f)-distance);

            if (showText) {
                var textColor = new Color(40, 40, 40, 210);
                batch.DrawString(_font, battleText, player_position + battleTextSize, textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                batch.DrawString(_font, discoverText, player_position + discoverTextSize, textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                batch.DrawString(_font, returnText, player_position + returnTextSize, textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                batch.DrawString(_font, newAntText, player_position + newAntTextSize, textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }


            distance = _scene.Height / 15;
            PlayerInput input = _handler.GetPlayer(index).GetPlayerInput();

            Color pressedColor = new Color(255, 255, 255, 100);

            int size = _scene.Height / 20;

            Func<string, Rectangle> buttonSource;
            var controlTexture = _gamepadButtons;
            if (isPlayerOnKeyboard) {
                controlTexture = _keyboardButtons;
                if (index == PlayerIndex.One) {
                    buttonSource = getKeyboardButtonSource1;
                } else {
                    buttonSource = getKeyboardButtonSource2;
                }
            } else {
                buttonSource = getGamepadButtonSource;
            }

            Rectangle newAntRectangle = new Rectangle((int)(player_position.X - size / 2f), (int)(player_position.Y - size / 2f - distance), size, size);
            batch.Draw(controlTexture, newAntRectangle, buttonSource("y"), input.IsNewInsectPressed() ? pressedColor : transparency );
            Rectangle discoverRectangle = new Rectangle((int)(player_position.X - size / 2f), (int)(player_position.Y - size / 2f + distance), size, size);
            batch.Draw(controlTexture, discoverRectangle, buttonSource("a"), input.IsDiscoverPressed() ? pressedColor : transparency);
            Rectangle returnRectangle = new Rectangle((int)(player_position.X - size / 2f - distance), (int)(player_position.Y - size / 2f), size, size);
            batch.Draw(controlTexture, returnRectangle, buttonSource("x"), input.IsReturnPressed() ? pressedColor : transparency);
            Rectangle battleRectangle = new Rectangle((int)(player_position.X - size / 2f + distance), (int)(player_position.Y - size / 2f), size, size);
            batch.Draw(controlTexture, battleRectangle, buttonSource("b"), input.IsFightPressed() ? pressedColor : transparency);
        }

        private Rectangle getGamepadButtonSource(string button) {
            var buttonSize = new Point(_gamepadButtons.Width / 2, _gamepadButtons.Height / 2);
            switch (button)
            {
                case "a":
                    return new Rectangle(Point.Zero, buttonSize);
                case "b":
                    return new Rectangle(new Point(buttonSize.X, 0), buttonSize);
                case "y":
                    return new Rectangle(buttonSize, buttonSize);
                case "x":
                    return new Rectangle(new Point(0, buttonSize.Y), buttonSize);
                
                default:
                    return new Rectangle(Point.Zero, buttonSize);
            } 
        }

        private Rectangle getKeyboardButtonSource2(string button) {
            var buttonSize = new Point(_keyboardButtons.Width / 4, _keyboardButtons.Height / 2);
            switch (button)
            {
                case "a":
                    return new Rectangle(new Point(buttonSize.X*2, 0), buttonSize);
                case "b":
                    return new Rectangle(new Point(buttonSize.X*2, buttonSize.Y), buttonSize);
                case "y":
                    return new Rectangle(new Point(buttonSize.X*3, buttonSize.Y), buttonSize);
                case "x":
                    return new Rectangle(new Point(buttonSize.X*3, 0), buttonSize);
                
                default:
                    return new Rectangle(Point.Zero, buttonSize);
            } 
        } 
        
        private Rectangle getKeyboardButtonSource1(string button) {
            var buttonSize = new Point(_keyboardButtons.Width / 4, _keyboardButtons.Height / 2);

            switch (button) {
                case "a":
                    return new Rectangle(Point.Zero, buttonSize);
                case "b":
                    return new Rectangle(new Point(0, buttonSize.Y), buttonSize);
                case "y":
                    return new Rectangle(new Point(buttonSize.X, buttonSize.Y), buttonSize);
                case "x":
                    return new Rectangle(new Point(buttonSize.X, 0), buttonSize);
                default:
                    return new Rectangle(Point.Zero, buttonSize);

            }
        }

    }
}

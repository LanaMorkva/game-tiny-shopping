using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Data;

namespace TinyShopping.Game {

    internal class TutorialUIController : UIController {

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
        private SplitScreenHandler _handler;

        private Scene _scene;
        private MenuInput _playerOne;

        private bool _selectPressed;

        private UIInsectController _insectController;

        public TutorialUIController(GraphicsDevice device, SplitScreenHandler handler, Scene scene): 
        base(device, handler, scene) {
            _handler = handler;
            _scene = scene;
            _playerOne = CreateMenuInput(PlayerIndex.One);
            _insectController = new UIInsectController(handler);
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

            _insectController.LoadContent(content);
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            if (_scene.gameState == GameState.Ended && _playerOne.IsSelectPressed()) {
                _selectPressed = true;
            } else if (_selectPressed) {
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
            _insectController.Draw(batch, gameTime);
            if (_scene.gameState == GameState.Ended) {
                DrawReturnMessage(batch);
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

        public void DrawString(SpriteBatch batch, String text, Vector2 position, float scale) {
            Vector2 origin = _font.MeasureString(text) / 2;
            batch.DrawString(_font, text, position, _textColor, 0, origin, scale, SpriteEffects.None, 0); // scale used to be 0.95f
        }
        public void DrawBoldString(SpriteBatch batch, String text, Vector2 position, float scale, float border = .005f) {
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
    }
}

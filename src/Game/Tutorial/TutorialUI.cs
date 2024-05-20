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
        TutorialScene.TutorialPhase _tutorialPhase = TutorialScene.TutorialPhase.None;
        double _runtimeS = 0;
        double _tutorialPhaseStartedS = 0;

        public TutorialUIController(GraphicsDevice device, SplitScreenHandler handler, Scene scene): 
        base(device, handler, scene, null) {
            _handler = handler;
            _scene = scene;
            _playerOne = CreateMenuInput(PlayerIndex.One);
            _insectController = new UIInsectController(handler);
        }

        public void SetTutorialPhase(TutorialScene.TutorialPhase phase, double seconds) {
            _tutorialPhase = phase;
            _tutorialPhaseStartedS = seconds;
        }

        public override void Update(GameTime gameTime)  {
            _runtimeS += gameTime.ElapsedGameTime.TotalSeconds;
            if (_scene.gameState == GameState.Ended && _playerOne.IsSelectPressed()) {
                _selectPressed = true;
            } else if (_selectPressed) {
                _selectPressed = false;
                _scene.LoadMainMenu();
            }
        }

        public void Draw(SpriteBatch batch, GameTime gameTime) {
            DrawBorder(batch);
            DrawStatistics(batch);
            _insectController.Draw(batch, gameTime);

            if (_tutorialPhase >= TutorialScene.TutorialPhase.CollectFood) {
                if (_tutorialPhase == TutorialScene.TutorialPhase.CollectFood && (_runtimeS - _tutorialPhaseStartedS) < 10) {
                    var buttonColor = new Color(122, 119, 110, 200);
                    var player1Pos = _handler.GetPlayerPosition(PlayerIndex.One);
                    var player2Pos = _handler.GetPlayerPosition(PlayerIndex.Two);
                    DrawCursorExplanations(batch, player1Pos, buttonColor, PlayerIndex.One, true, _handler.IsPlayerKeyboard(PlayerIndex.One));
                    DrawCursorExplanations(batch, player2Pos, buttonColor, PlayerIndex.Two, true, _handler. IsPlayerKeyboard(PlayerIndex.Two));
                } else {
                    DrawControls(batch);
                }
            }

            if (_scene.gameState == GameState.Ended) {
                DrawReturnMessage(batch);
            }
#if DEBUG
            int fps = (int) Math.Round((1000 / gameTime.ElapsedGameTime.TotalMilliseconds));
            DrawString(batch, "FPS: " + fps.ToString(), new Vector2(80, 20), _fontScale);
#endif
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
    }
}

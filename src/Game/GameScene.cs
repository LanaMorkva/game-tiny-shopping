﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.Game {

    public class GameScene : TinyShopping.Scene {
        private UIController _ui;

        private SoundController _sound;

        private SplitScreenHandler _splitScreenHandler;

        private Rectangle _player1Area;

        private Rectangle _player2Area;

        private SelectMenu _pauseMenu;

        public GameScene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game, SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
        }

        public override void Initialize() {
            base.Initialize();
            _player1Area = new Rectangle(0, 0, Width / 2, Height);
            _player2Area = new Rectangle(Width / 2, 0, Width / 2, Height);
            _splitScreenHandler = new SplitScreenHandler(_player1Area, _player2Area, GraphicsDevice, this);
            _splitScreenHandler.Initialize();
            _ui = new UIController(GraphicsDevice, _splitScreenHandler, this);
            _sound = new SoundController(this);

            var menuRegion = new Rectangle(0, 0, Width, Height);
            var menuItemSize = new Vector2((int)(Width / 2.8), Height / 10);

            Rectangle explanationRegion = new Rectangle(50, Height - 150, 300, 100);
            List<MenuExplanation> explanations = new List<MenuExplanation> {
                new("<A>", "Select", Color.Green),
                new("<B>", "Resume Game", Color.Red)
            };
            _pauseMenu = new SelectMenu(menuRegion, menuItemSize, ResumeGame, explanationRegion, explanations);
            _pauseMenu.AddItem(new MenuItem("Resume", ResumeGame));
            _pauseMenu.AddItem(new MenuItem("Exit Game", LoadMainMenu));
        }

        public override void LoadContent() {
            _splitScreenHandler.LoadContent(Content);
            _ui.LoadContent(Content);
            _sound.LoadContent(Content);

            _pauseMenu.LoadContent(Content);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _splitScreenHandler.UnloadContent(Content);
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime) {
            if (!IsOver && IsStarted && !IsPaused) {
                _splitScreenHandler.Update(gameTime, this);
            }

            if (!IsOver && IsStarted && IsPaused) {
                _pauseMenu.Update(gameTime);
            }

            _ui.Update(gameTime);
            _sound.Update(gameTime, _ui);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();
            
            Viewport original = GraphicsDevice.Viewport;
            _splitScreenHandler.Draw(SpriteBatch, gameTime);
            _ui.Draw(SpriteBatch, gameTime);
            GraphicsDevice.Viewport = original;

            if (IsPaused) {
                SpriteBatch.FillRectangle(new Rectangle(0, 0, Width, Height), new Color(122, 119, 110, 120), 0);
                _pauseMenu.Draw(SpriteBatch);
            }
            
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

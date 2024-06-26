﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TinyShopping.SettingsMenu {

    public class Scene : TinyShopping.Scene {
        private SelectMenu _selectMenu;

        private Texture2D _imageTexture;

        private SpriteFont _font;

        private Vector2 _titleLocation;

        private Rectangle _imageRegion;
        private Rectangle _titleRegion;

        private SoundEffectInstance _backgroundSong;
        private SoundEffectInstance _supermarketNoiseInstance;

        private Color _backColor = new Color(211, 237, 150);

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game,
                    SettingsHandler settingsHandler) :
            base(content, graphics, manager, game, settingsHandler) {
        }

        public override void Initialize() {
            int menuPosY = (int)(Height / 10);
            int menuW = (int)(Width / 2.3);
            var menuRegion = new Rectangle(menuW / 10, menuPosY, menuW, Height - menuPosY);
            var menuItemSize = new Vector2((int)(Width / 2.2), Height / 10);

            _titleLocation = new Vector2(menuW / 8, (int)(menuPosY / 3));

            _titleRegion = new Rectangle(menuW / 8, (int)(menuPosY / 1.5), menuW, menuPosY);
            _imageRegion = new Rectangle((int)(menuW / 1.5), menuPosY / 3, (int)(Width - menuW / 1.5),
                Height - menuPosY / 3);

            Rectangle explanationRegion = new Rectangle(50, Height - 150, 300, 100);
            List<MenuExplanation> explanations = new List<MenuExplanation> {
                new("<A>", "Change Setting", Color.Green),
                new("<B>", "Back", Color.Red)
            };
            _selectMenu = new SelectMenu(menuRegion, menuItemSize, GoBack, explanationRegion, explanations, this.SettingsHandler.SoundPlayer);
            _selectMenu.AddItem(new MenuItemBool("Music", ChangeMusicSettings, SettingsHandler.settings.music));
            _selectMenu.AddItem(new MenuItemInt("Music Volume", DecreaseMusicVolumeSettings, IncreaseMusicVolumeSettings, SettingsHandler.settings.musicVolume, 0, 100));
            _selectMenu.AddItem(new MenuItemBool("Sound Effects", ChangeSoundEffectsSettings, SettingsHandler.settings.soundEffects));
            _selectMenu.AddItem(new MenuItemInt("Effects Volume", DecreaseEffectsVolumeSettings, IncreaseEffectsVolumeSettings, SettingsHandler.settings.effectsVolume, 0, 100));
            _selectMenu.AddItem(new MenuItemBool("Fullscreen", ChangeFullScreenSettings, SettingsHandler.settings.fullScreen));
            base.Initialize();
        }

        public override void LoadContent() {
            _imageTexture = Content.Load<Texture2D>("main_menu/teaser");
            _font = Content.Load<SpriteFont>("fonts/General");
            _backgroundSong = Content.Load<SoundEffect>("songs/basic_supermarket").CreateInstance();
            _supermarketNoiseInstance = Content.Load<SoundEffect>("sounds/supermarket_atmosphere").CreateInstance();
            _selectMenu.LoadContent(Content);

            _supermarketNoiseInstance.IsLooped = true;
            _backgroundSong.IsLooped = true;
            SettingsHandler.SoundPlayer.playSong(_backgroundSong, 1f);
            SettingsHandler.SoundPlayer.playSong(_supermarketNoiseInstance, 1f);
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            Content.UnloadAsset("main_menu/teaser");
            Content.UnloadAsset("fonts/General");
            _selectMenu.UnloadContent(Content);
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime) {
            _selectMenu.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();

            var backRegion = new RectangleF(0, 0, Width, Height);
            SpriteBatch.FillRectangle(backRegion, _backColor);
            // Draw menu texture
            SpriteBatch.Draw(_imageTexture, _imageRegion, new Rectangle(40, 70, 535, 390), Color.White);
            _selectMenu.Draw(SpriteBatch);

            SpriteBatch.DrawString(_font, "Settings", _titleLocation, Color.Coral, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            SpriteBatch.End();
            base.Draw(gameTime);
        }

        public void ChangeMusicSettings() {
            SettingsHandler.ToggleMusic();
        }

        public void ChangeSoundEffectsSettings() {
            SettingsHandler.ToggleSoundEffects();
        }

        public void ChangeFullScreenSettings() {
            SettingsHandler.ToggleFullScreen(GraphicsDeviceManager);
        }

        public void IncreaseMusicVolumeSettings() {
            SettingsHandler.ChangeMusicVolumeSettings(SettingsHandler.GetMusicVolume() + 1);
        }

        public void DecreaseMusicVolumeSettings() {
            SettingsHandler.ChangeMusicVolumeSettings(SettingsHandler.GetMusicVolume() - 1);
        }

        public void DecreaseEffectsVolumeSettings() {
            SettingsHandler.ChangeEffectsVolumeSettings(SettingsHandler.GetEffectsVolume() - 1);
        }

        public void IncreaseEffectsVolumeSettings() {
            SettingsHandler.ChangeEffectsVolumeSettings(SettingsHandler.GetEffectsVolume() + 1);
        }
    }
}

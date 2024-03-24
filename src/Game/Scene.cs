using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TinyShopping.Game {

    public class Scene : TinyShopping.Scene {

        private SpriteBatch _spriteBatch;

        private static Texture2D _statsTexture;

        private Texture2D _antTexture;

        private Texture2D _appleTexture;

        private SpriteFont _font;

        public static int STAT_OFFSET = 70;

        private World _world;

        private Colony _colony1;

        private Colony _colony2;

        private Player _player1;

        private Player _player2;

        private PheromoneHandler _pheromoneHandler;

        private UIController _ui;

        private FruitHandler _fruitHandler;

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game) :
            base(content, graphics, manager, game) {
        }

        public override void Initialize() {
            _world = new World();
            _pheromoneHandler = new PheromoneHandler(_world);
            _fruitHandler = new FruitHandler(_world);
            _ui = new UIController();
            base.Initialize();
        }

        /// <summary>
        public override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _world.LoadContent(Content);
            _world.createWorld(new Rectangle(0, STAT_OFFSET, GraphicsDeviceManager.PreferredBackBufferWidth,
                GraphicsDeviceManager.PreferredBackBufferHeight - STAT_OFFSET));

            createStatisticsTexture();

            _font = Content.Load<SpriteFont>("arial");
            _appleTexture = Content.Load<Texture2D>("apple");
            _antTexture = Content.Load<Texture2D>("ant_texture");

            _colony1 = new Colony(_world.GetTopLeftOfTile(5, 0), 180, _world, _pheromoneHandler, _fruitHandler, _world.GetTopLeftOfTile(5, 3), 0);
            _colony1.Initialize();
            _colony2 = new Colony(_world.GetTopLeftOfTile(57, 35), 270, _world, _pheromoneHandler, _fruitHandler, _world.GetTopLeftOfTile(54, 35), 1);
            _colony2.Initialize();
            _colony1.LoadContent(Content);
            _colony2.LoadContent(Content);

            PlayerInput input1 = createPlayerInput(PlayerIndex.One);
            _player1 = new Player(_world, _pheromoneHandler, input1, _colony1, 0, _world.GetTopLeftOfTile(5, 3));
            _player1.LoadContent(Content);

            PlayerInput input2 = createPlayerInput(PlayerIndex.Two);
            _player2 = new Player(_world, _pheromoneHandler, input2, _colony2, 1, _world.GetTopLeftOfTile(54, 35));
            _player2.LoadContent(Content);

            _pheromoneHandler.LoadContent(Content);
            _ui.LoadContent(Content);
            _fruitHandler.LoadContent(Content);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            _colony1.Update(gameTime);
            _colony2.Update(gameTime);
            _player1.Update(gameTime);
            _player2.Update(gameTime);
            _pheromoneHandler.Update(gameTime);
            _ui.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            _spriteBatch.Begin();
            _world.Draw(_spriteBatch, gameTime);
            _colony1.Draw(_spriteBatch, gameTime);
            _colony2.Draw(_spriteBatch, gameTime);
            _player1.Draw(_spriteBatch, gameTime);
            _player2.Draw(_spriteBatch, gameTime);
            _pheromoneHandler.Draw(_spriteBatch, gameTime);
            _fruitHandler.Draw(_spriteBatch, gameTime);
            _ui.Draw(_spriteBatch, gameTime);

            drawStatistics();

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void createStatisticsTexture() {
            _statsTexture = new Texture2D(GraphicsDevice, _world.WorldRegion.Width, STAT_OFFSET);
            Color[] data = new Color[_world.WorldRegion.Width * STAT_OFFSET];
            for (int i = 0; i < data.Length; i++) {
                data[i] = new Color(252, 239, 197);
            }
            _statsTexture.SetData(data);
        }

        private void drawStatistics() {
            int offset = 15;
            int textureSize = 40;
            int offsetText = 35;

            int offsetL = (int)_world.WorldRegion.X + offset;
            int offsetR = (int)(_world.WorldRegion.X + _world.WorldRegion.Width) - (offset + textureSize);

            _spriteBatch.Draw(_statsTexture, new Vector2(_world.WorldRegion.X, 0), Color.White);

            _spriteBatch.Draw(_antTexture, new Rectangle(offsetL, offset, textureSize, textureSize), Color.White);
            _spriteBatch.Draw(_antTexture, new Rectangle(offsetR, offset, textureSize, textureSize), Color.White);

            offsetL += textureSize + 5;
            offsetR -= 5;
            String AntsNum1 = "x" + _colony1.AntsNum;
            Vector2 sizeAnts1 = _font.MeasureString(AntsNum1) / 2;
            _spriteBatch.DrawString(_font, AntsNum1, new Vector2(offsetL, offsetText), Color.Black, 0, sizeAnts1, 0.8f, SpriteEffects.None, 0);

            String AntsNum2 = _colony2.AntsNum + "x";
            Vector2 sizeAnts2 = _font.MeasureString(AntsNum2) / 2;
            _spriteBatch.DrawString(_font, AntsNum2, new Vector2(offsetR, offsetText), Color.Black, 0, sizeAnts2, 0.8f, SpriteEffects.None, 0);

            offsetL += 20 + (int)sizeAnts1.X;
            offsetR -= (20 + (int)sizeAnts2.X + textureSize);
            _spriteBatch.Draw(_appleTexture, new Rectangle(offsetL, offset, textureSize, textureSize), Color.White);
            _spriteBatch.Draw(_appleTexture, new Rectangle(offsetR, offset, textureSize, textureSize), Color.White);

            offsetL += textureSize + 15;
            offsetR -= 15;
            String FruitsNum1 = "x" + _colony1.FruitsNum;
            Vector2 sizeFruits1 = _font.MeasureString(FruitsNum1) / 2;
            _spriteBatch.DrawString(_font, FruitsNum1, new Vector2(offsetL, offsetText), Color.Black, 0, sizeFruits1, 0.8f, SpriteEffects.None, 0);

            String FruitsNum2 = _colony2.FruitsNum + "x";
            Vector2 sizeFruits2 = _font.MeasureString(FruitsNum2) / 2;
            _spriteBatch.DrawString(_font, FruitsNum2, new Vector2(offsetR, offsetText), Color.Black, 0, sizeFruits2, 0.8f, SpriteEffects.None, 0);
        }

        private PlayerInput createPlayerInput(PlayerIndex playerIndex) {
            GamePadState state = GamePad.GetState(playerIndex);
            if (state.IsConnected) {
                return new GamePadInput(playerIndex);
            } 
            return new KeyboardInput(playerIndex);

        }
    }
}

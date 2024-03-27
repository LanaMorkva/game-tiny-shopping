using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TinyShopping.Game {

    public class Scene : TinyShopping.Scene {

        private SpriteBatch _spriteBatch;

        public static int STAT_OFFSET = 70;

        private InsectHandler _insectHandler;

        private World _world;

        private Player _player1;

        private Player _player2;

        private PheromoneHandler _pheromoneHandler;

        private UIController _ui;

        private FruitHandler _fruitHandler;

        private Rectangle _player1Area;

        private Rectangle _player2Area;

        private Rectangle _statsArea;

        public Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game) :
            base(content, graphics, manager, game) {
        }

        public override void Initialize() {
            int width = GraphicsDeviceManager.PreferredBackBufferWidth;
            int height = GraphicsDeviceManager.PreferredBackBufferHeight;
            _statsArea = new Rectangle(0, 0, width, STAT_OFFSET);
            _player1Area = new Rectangle(0, STAT_OFFSET, width / 2, height - STAT_OFFSET);
            _player2Area = new Rectangle(width / 2, STAT_OFFSET, width / 2, height - STAT_OFFSET);
            _world = new World(_player1Area, _player2Area, GraphicsDevice);
            _pheromoneHandler = new PheromoneHandler(_world);
            _fruitHandler = new FruitHandler(_world);
            _insectHandler = new InsectHandler(_world, _pheromoneHandler, _fruitHandler);
            _ui = new UIController(_statsArea, GraphicsDevice, _insectHandler);
            base.Initialize();
        }

        public override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _world.LoadContent(Content);
            //_world.createWorld(new Rectangle(0, STAT_OFFSET, GraphicsDeviceManager.PreferredBackBufferWidth,
                //GraphicsDeviceManager.PreferredBackBufferHeight - STAT_OFFSET));
            _insectHandler.LoadContent(Content);

            PlayerInput input1 = CreatePlayerInput(PlayerIndex.One);
            _player1 = new Player(_world, _pheromoneHandler, input1, _insectHandler, 0, _world.GetTopLeftOfTile(5, 3));
            _player1.LoadContent(Content);

            PlayerInput input2 = CreatePlayerInput(PlayerIndex.Two);
            _player2 = new Player(_world, _pheromoneHandler, input2, _insectHandler, 1, _world.GetTopLeftOfTile(54, 35));
            _player2.LoadContent(Content);

            _pheromoneHandler.LoadContent(Content);
            _ui.LoadContent(Content);
            _fruitHandler.LoadContent(Content);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            _insectHandler.Update(gameTime);
            _player1.Update(gameTime);
            _player2.Update(gameTime);
            _pheromoneHandler.Update(gameTime);
            _ui.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            _spriteBatch.Begin();
            _world.DrawFloor(_spriteBatch, gameTime);
            
            _pheromoneHandler.Draw(_spriteBatch, gameTime);
            _world.DrawObjects(_spriteBatch, gameTime);
            _insectHandler.Draw(_spriteBatch, gameTime);
            _player1.Draw(_spriteBatch, gameTime);
            _player2.Draw(_spriteBatch, gameTime);
            _fruitHandler.Draw(_spriteBatch, gameTime);
            _ui.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private PlayerInput CreatePlayerInput(PlayerIndex playerIndex) {
            GamePadState state = GamePad.GetState(playerIndex);
            if (state.IsConnected) {
                return new GamePadInput(playerIndex);
            } 
            return new KeyboardInput(playerIndex);

        }
    }
}

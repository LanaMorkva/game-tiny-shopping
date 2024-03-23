using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    public class Scene : TinyShopping.Scene {

        private SpriteBatch _spriteBatch;

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
            _world = new World(GraphicsDeviceManager);
            _pheromoneHandler = new PheromoneHandler(_world);
            _fruitHandler = new FruitHandler(_world);
            _ui = new UIController();
            base.Initialize();
        }

        public override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _world.LoadContent(Content);

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

            _spriteBatch.End();
            base.Draw(gameTime);
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

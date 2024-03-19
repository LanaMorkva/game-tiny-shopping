using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    public class Scene : global::TinyShopping.Scene {

        private SpriteBatch _spriteBatch;

        private World _world;

        private Colony _colony1;

        private Colony _colony2;

        private Player _player1;

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
            _colony1 = new Colony(new Vector2(300, 300), _world, _pheromoneHandler, _fruitHandler);
            _colony1.Initialize();
            _colony2 = new Colony(new Vector2(1200, 800), _world, _pheromoneHandler, _fruitHandler);
            _colony2.Initialize();
            _player1 = new Player(_world, _pheromoneHandler);
            _ui = new UIController();
            base.Initialize();
        }

        public override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _world.LoadContent(Content);
            _colony1.LoadContent(Content);
            _colony2.LoadContent(Content);
            _player1.LoadContent(Content);
            _pheromoneHandler.LoadContent(Content);
            _ui.LoadContent(Content);
            _fruitHandler.LoadContent(Content);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {
            _colony1.Update(gameTime);
            _colony2.Update(gameTime);
            _player1.Update(gameTime);
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
            _pheromoneHandler.Draw(_spriteBatch, gameTime);
            _fruitHandler.Draw(_spriteBatch, gameTime);
            _ui.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

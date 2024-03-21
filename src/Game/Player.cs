using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping.Game {

    /// <summary>
    /// The Player handles user input and pheromone placement
    /// </summary>
    internal class Player {

        private static readonly int SIZE = 50;

        private static readonly int SPEED = 200;

        private World _world;

        private Vector2 _position;

        private Texture2D _texture;

        private bool _discoverPressed;

        private bool _returnPressed;

        private bool _newInsectPressed;

        private PheromoneHandler _handler;

        private PlayerInput _input;

        private Colony _colony;

        private int _id;

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="world">The world to exist in.</param>
        /// <param name="handler">The pheromone handler to use.</param>
        /// <param name="input">The player input to use.</param>
        /// <param name="colony">The colony controlled by this player.</param>
        /// <param name="id">The palyer id, 0 or 1.</param>
        public Player(World world, PheromoneHandler handler, PlayerInput input, Colony colony, int id) {
            _world = world;
            _position = new Vector2(300, 300);
            _handler = handler;
            _input = input;
            _colony = colony;
            _id = id;
        }

        /// <summary>
        /// Loads the necessary files from disk.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content) {
            _texture = content.Load<Texture2D>("crosshair");
        }

        /// <summary>
        /// Draws the player cursor.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
            spriteBatch.Draw(_texture, new Rectangle((int)(_position.X - SIZE/2f), (int)(_position.Y - SIZE/2f), SIZE, SIZE), Color.White);
        }

        /// <summary>
        /// Reads user input and updates the cursor position.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime) {
            Vector2 motion = _input.GetMotion();
            if (_input.IsDiscoverPressed()) {
                _discoverPressed = true;
            }
            else if (_discoverPressed) {
                _discoverPressed = false;
                _handler.AddPheromone(_position, gameTime, PheromoneType.DISCOVER, _id);
            }
            if (_input.IsReturnPressed()) {
                _returnPressed = true;
            }
            else if (_returnPressed) {
                _returnPressed = false;
                _handler.AddPheromone(_position, gameTime, PheromoneType.RETURN, _id);
            }
            if (_input.IsNewInsectPressed()) {
                _newInsectPressed = true;
            }
            else if (_newInsectPressed) {
                _newInsectPressed = false;
                _colony.BuyNewInsect();
            }
            _position.X += motion.X * (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
            _position.Y += motion.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
        }
    }
}

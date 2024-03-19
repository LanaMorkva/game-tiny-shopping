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

        private static readonly int SPEED = 150;

        private World _world;

        private Vector2 _position;

        private Texture2D _texture;

        private bool _discoverPressed;

        private bool _returnPressed;

        private PheromoneHandler _handler;

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="world">The world to exist in.</param>
        /// <param name="handler">The pheromone handler to use.</param>
        public Player(World world, PheromoneHandler handler) {
            _world = world;
            _position = new Vector2(300, 300);
            _handler = handler;
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
            KeyboardState state = Keyboard.GetState();
            Vector2 motion = new Vector2(0, 0);
            if (state.IsKeyDown(Keys.A)) {
                motion.X = -1;
            }
            if (state.IsKeyDown(Keys.D)) {
                motion.X = +1;
            }
            if (state.IsKeyDown(Keys.W)) {
                motion.Y = -1;
            }
            if (state.IsKeyDown(Keys.S)) {
                motion.Y = +1;
            }
            if (state.IsKeyDown(Keys.J)) {
                _discoverPressed = true;
            }
            else if (_discoverPressed) {
                _discoverPressed = false;
                _handler.AddPheromone(_position, gameTime, PheromoneType.DISCOVER);
            }
            if (state.IsKeyDown(Keys.K)) {
                _returnPressed = true;
            }
            else if (_returnPressed) {
                _returnPressed = false;
                _handler.AddPheromone(_position, gameTime, PheromoneType.RETURN);
            }
            if (motion.LengthSquared()  > 0) {
                motion.Normalize();
            }
            _position.X += motion.X * (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
            _position.Y += motion.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
        }
    }
}

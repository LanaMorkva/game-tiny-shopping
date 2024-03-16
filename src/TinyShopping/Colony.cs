using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GameLab.TinyShopping {

    internal class Colony {

        private Vector2 _spawn;

        private World _world;

        private List<Insect> _insects = new List<Insect>();

        /// <summary>
        /// Creates a new colony.
        /// </summary>
        /// <param name="spawn">The spawn point of the insects.</param>
        /// <param name="world">The world to live in.</param>
        public Colony(Vector2 spawn, World world) {
            _spawn = spawn;
            _world = world;
        }

        /// <summary>
        /// Initializes the colony and it's insects.
        /// </summary>
        public void Initialize() {
            for (int i = 0; i < 20; ++i) {
                _insects.Add(new Insect(_world, _spawn));
            }
        }

        /// <summary>
        /// Loads the necessary data from disk.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content) {
            Texture2D texture = content.Load<Texture2D>("ant_texture");
            foreach (Insect insect in _insects) {
                insect.LoadContent(content, texture);
            }
        }

        /// <summary>
        /// Updates the insects' position.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            foreach (Insect insect in _insects) {
                insect.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws the insects.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw to.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
            foreach (Insect insect in _insects) {
                insect.Draw(spriteBatch, gameTime);
            }
        }
    }
}

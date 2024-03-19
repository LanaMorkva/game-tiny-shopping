using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyShopping {

    public abstract class Scene {

        protected ContentManager Content { get; private set; }

        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        protected SpriteBatch SpriteBatch { get; private set; }

        protected Renderer Game { get; private set; }

        protected Scene(ContentManager content, GraphicsDevice graphics, GraphicsDeviceManager manager, Renderer game) {
            Content = content;
            GraphicsDevice = graphics;
            GraphicsDeviceManager = manager;
            Game = game;
        }

        public virtual void Initialize() {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public virtual void LoadContent() {

        }

        public virtual void Update(GameTime gameTime) {

        }

        public virtual void Draw(GameTime gameTime) {

        }
    }
}

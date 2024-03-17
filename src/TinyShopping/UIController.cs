using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLab.TinyShopping {

    internal class UIController {

        private SpriteFont _font;

        public void LoadContent(ContentManager content) {
            _font = content.Load<SpriteFont>("Arial");
        }

        public void Update(GameTime gameTime) {

        }

        public void Draw(SpriteBatch batch, GameTime gameTime) {
#if DEBUG
            int fps = (int) Math.Round((1000 / gameTime.ElapsedGameTime.TotalMilliseconds));
            batch.DrawString(_font, "FPS: " + fps.ToString(), new Vector2(0, 0), Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
#endif
        }
    }
}

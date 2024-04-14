using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyShopping.Game
{
    // https://www.youtube.com/watch?v=hm4PkqS2bqY
    internal class AnimationManager
    {
        private readonly Dictionary<object, Animation> _anims = new Dictionary<object, Animation>();
        private object _lastKey;

        public void AddAnimation(object key, Animation animation)
        {
            _anims.Add(key, animation);
            _lastKey ??= key;
        }

        public void Update(object key, GameTime gameTime)
        {
            if (_anims.TryGetValue(key, out Animation value))
            {
                value.Start();
                _anims[key].Update(gameTime);
                _lastKey = key;
            }
            else
            {
                _anims[_lastKey].Stop();
                _anims[_lastKey].Reset();
            }
        }

        public void Draw(SpriteBatch batch, GameTime gameTime, Rectangle destination, Vector2 origin)
        {
            _anims[_lastKey].Draw(batch, gameTime, destination, origin);
        }
    }
}

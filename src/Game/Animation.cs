using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace TinyShopping.Game
{
    internal class Animation
    {
        private readonly Texture2D _texture;
        private readonly List<Rectangle> _sourceRectangles = new List<Rectangle>();
        private readonly int _frames;
        private int _frame;
        private readonly float _frameTime;
        private float _frameTimeLeft;
        private bool _active = true;

        public Animation(Texture2D texture, int framesX, int framesY, float frameTime, int row = 1)
        {
            _texture = texture;
            _frameTime = frameTime;
            _frameTimeLeft = _frameTime;
            _frames = framesX;
            var frameWidth = _texture.Width / framesX;
            var frameHeight = _texture.Height / framesY;

            for (int i = 0; i < _frames; i++)
            {
                _sourceRectangles.Add(new(i * frameWidth, (row - 1) * frameHeight, frameWidth, frameHeight));
            }
        }

        public void Stop()
        {
            _active = false;
        }

        public void Start()
        {
            _active = true;
        }

        public void Reset()
        {
            _frame = 0;
            _frameTimeLeft = _frameTime;
        }

        public void Update(GameTime gameTime)
        {
            if (!_active) return;

            _frameTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_frameTimeLeft <= 0)
            {
                _frameTimeLeft += _frameTime;
                _frame = (_frame + 1) % _frames;
            }
        }

        public void Draw(SpriteBatch batch, GameTime gameTime, Rectangle destination, Vector2 origin)
        {
            batch.Draw(_texture, destination, _sourceRectangles[_frame], Color.White, 0, origin, SpriteEffects.None, 1);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;


namespace TinyShopping.Game {

    internal class Pheromone {

        public Vector2 Position { private set; get; }

        private Texture2D _texture;
        private ParticleEffect _particleEffect;
        private Color _color;

        public int Priority { private set; get; }

        public int Range { private set; get; }

        public int Duration { private set; get; }

        public PheromoneType Type { private set; get; }

        public int Owner { private set; get; }

        /// <summary>
        /// Creates a new pheromone spot.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="world">The world to exist in.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="duration">The duration of the pheromone. This will decrease for each passing milisecond.</param>
        /// <param name="range">The pheromone effect range.</param>
        /// <param name="type">The pheromone type.</param>
        /// <param name="owner">The player placing the pheromone.</param>
        public Pheromone(Vector2 position, Texture2D texture, World world, int priority, int duration, int range, PheromoneType type, int owner) {
            Position = position;
            _texture = texture;
            Priority = priority;
            Type = type;
            Owner = owner;
            Range = range;
            Duration = duration;
            switch (type) {
                case PheromoneType.RETURN:
                    _color = Color.Blue;
                    break;
                case PheromoneType.FIGHT:
                    _color = Color.Red;
                    break;
                default:
                    _color = Color.Green;
                    break;
            }

            
            TextureRegion2D textureRegion = new TextureRegion2D(_texture);
            _particleEffect = new ParticleEffect() {
                Position = position,
                Emitters = new List<ParticleEmitter> {
                    new ParticleEmitter(textureRegion, 100, System.TimeSpan.FromMilliseconds(duration),
                        Profile.Circle(range / 4, Profile.CircleRadiation.None)) {
                        Parameters = new ParticleReleaseParameters {
                            Speed = new Range<float>(0f, 70f),
                            Quantity = 3,
                            Scale = new Range<float>(0.1f, 0.5f),
                            Opacity = new Range<float>(0.5f, 0.5f),
                        },
                        Modifiers = { new AgeModifier { Interpolators = {
                            new ColorInterpolator {StartValue = _color.ToHsl(), EndValue = _color.ToHsl()} } },
                            new RotationModifier {RotationRate = -2.1f},
                            new CircleContainerModifier {Radius = range, Inside = true},
                        }
                    }, 
                    new ParticleEmitter(textureRegion, 500, System.TimeSpan.FromMilliseconds(500),
                        Profile.Ring(range, Profile.CircleRadiation.None)) {
                        Parameters = new ParticleReleaseParameters {
                            Speed = new Range<float>(0f, 5f),
                            Quantity = 30,
                            Scale = new Range<float>(0.1f, 0.5f)
                        },
                        Modifiers = {new AgeModifier { Interpolators = {
                            new ColorInterpolator {StartValue = Color.DarkGray.ToHsl(), EndValue = Color.DarkGray.ToHsl()} } },
                            new RotationModifier {RotationRate = -2.1f}}
                    }
                }
            };
        }


        public void Dispose() {
            _particleEffect.Dispose();
        }

        /// <summary>
        /// Draws the pheromone spot.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            batch.Draw(_particleEffect);
        }

        /// <summary>
        /// Updates the pheromone. Decreases the priority.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            Duration -= (int) Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}

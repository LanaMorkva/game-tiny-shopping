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

using PFPoint = TinyShopping.Game.Pathfinding.Point;

namespace TinyShopping.Game {

    internal class Pheromone {

        public Vector2 Position { private set; get; }

        private Texture2D _texture;
        private ParticleEffect _particleEffect;
        private ParticleEffect _trailEffect;
        private Color _color;

        private Dictionary<int, List<PFPoint>> _antPaths = new  Dictionary<int, List<PFPoint>>();

        public int Priority { private set; get; }

        public int Range { private set; get; }

        public int Duration { private set; get; }

        public PheromoneType Type { private set; get; }

        public int Owner { private set; get; }

        public bool IsPlayer { private set; get; }

        /// <summary>
        /// Creates a new pheromone spot.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="duration">The duration of the pheromone. This will decrease for each passing milisecond.</param>
        /// <param name="range">The pheromone effect range.</param>
        /// <param name="type">The pheromone type.</param>
        /// <param name="owner">The player placing the pheromone.</param>
        /// <param name="isPlayer">If the pheromone is placed by a player.</param>
        public Pheromone(Vector2 position, Texture2D texture, int priority, int duration, int range, PheromoneType type, int owner, bool isPlayer) {
            Position = position;
            _texture = texture;
            Priority = priority;
            Type = type;
            Owner = owner;
            Range = range;
            Duration = duration;
            IsPlayer = isPlayer;
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
            if (isPlayer) {
                CreateParticleEffects(position, duration, range);
            }
        }

        /// <summary>
        /// Creates the particle effects.
        /// </summary>
        /// <param name="position">The position to use.</param>
        /// <param name="duration">The duration of the pheromone.</param>
        /// <param name="range">The pheromone effect range.</param>
        private void CreateParticleEffects(Vector2 position, int duration, int range) {
            TextureRegion2D textureRegion = new TextureRegion2D(_texture);
            _particleEffect = new ParticleEffect() {
                Position = position,
                Emitters = new List<ParticleEmitter> {
                    new ParticleEmitter(textureRegion, 100, System.TimeSpan.FromMilliseconds(duration),
                        Profile.Circle(10, Profile.CircleRadiation.None)) {
                        Parameters = new ParticleReleaseParameters {
                            Speed = new Range<float>(0f, 10f),
                            Quantity = 3,
                            Scale = new Range<float>(0.1f, 0.5f),
                            Opacity = new Range<float>(0.5f, 0.5f),
                        },
                        Modifiers = { new AgeModifier { Interpolators = {
                            new ColorInterpolator {StartValue = _color.ToHsl(), EndValue = _color.ToHsl()} } },
                            new RotationModifier {RotationRate = -2.1f},
                            new CircleContainerModifier {Radius = 10, Inside = true},
                        }
                    },
                    new ParticleEmitter(textureRegion, 500, System.TimeSpan.FromSeconds(1),
                        Profile.Ring(range, Profile.CircleRadiation.None)) {
                        Parameters = new ParticleReleaseParameters {
                            Speed = new Range<float>(0f, 5f),
                            Quantity = 10,
                            Scale = new Range<float>(0.1f, 0.4f)
                        },
                        Modifiers = {new AgeModifier { Interpolators = {
                            new ColorInterpolator {StartValue = Color.DarkGray.ToHsl(), EndValue = Color.DarkGray.ToHsl()} } },
                            new RotationModifier {RotationRate = -2.1f}}
                    }
                }
            };

            _trailEffect = new ParticleEffect() {
                Emitters = new List<ParticleEmitter> {
                    new ParticleEmitter(textureRegion, 500, System.TimeSpan.FromSeconds(1),
                    Profile.Line(Vector2.Zero, 30)) {
                        Parameters = new ParticleReleaseParameters {
                                Speed = new Range<float>(0f, 5f),
                                Quantity = 3,
                                Scale = new Range<float>(0.1f, 0.2f)
                        },
                        Modifiers = { new AgeModifier { Interpolators = {
                            new ColorInterpolator {StartValue = _color.ToHsl(), EndValue = _color.ToHsl()} } },
                            new RotationModifier {RotationRate = -2.1f},
                        }
                    }
                }
            };
            _trailEffect.Emitters[0].AutoTrigger = false;
        }

        /// <summary>
        /// Add path from Pathfinder to render pheromone trail. Will add new key or update existing key
        /// </summary>
        public void AddPathForAnt(int insectHash, List<PFPoint> path) {
            _antPaths[insectHash] = path; // add or update the path
        }

        /// <summary>
        /// Remove path from Pathfinder
        /// </summary>
        public void RemovePathForAnt(int insectHash) {
            _antPaths.Remove(insectHash);
        }

        /// <summary>
        /// Unload all resources that were not loaded via ContentManager
        /// </summary>
        public void Dispose() {
            if (IsPlayer) {
                _particleEffect.Dispose();
                _trailEffect.Dispose();
            }
        }

        /// <summary>
        /// Draws the pheromone spot.
        /// </summary>
        /// <param name="handler">The split screen handler to use for rendering.</param>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime) {
            if (!IsPlayer) {
                int size = 20;
                Rectangle r = new Rectangle((int)Position.X - size / 2, (int)Position.Y - size / 2, size, size);
                Color c = new Color(_color, 0.05f);
                batch.Draw(_texture, r, c);
                return;
            }
            foreach(var path in _antPaths) {
                if (path.Value.Count == 0) {
                    continue;
                }

                Vector2 current = new Vector2(path.Value[0].X, path.Value[0].Y);
                for (int i = 1; i < path.Value.Count; i++) {
                    Vector2 p = new Vector2(path.Value[i].X, path.Value[i].Y);
                    LineSegment lineSegment = new LineSegment(current, p);
                    _trailEffect.Trigger(lineSegment);
                    current = p;
                }
            }
            
            batch.Draw(_trailEffect);
            batch.Draw(_particleEffect);
        }

        /// <summary>
        /// Updates the pheromone. Decreases the priority.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime) {
            if (IsPlayer) {
                _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                _trailEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            Duration -= (int) Math.Floor(gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}

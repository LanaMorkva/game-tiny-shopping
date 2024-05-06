using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using MonoGame.Extended;
using TinyShopping.Game.Pathfinding;
using PFPoint = TinyShopping.Game.Pathfinding.Point;


namespace TinyShopping.Game.AI {
    internal class AIHandler {
        private double _nextUpdateTime = 0;

        private Insect _insect;
        private Task[] _ais;

        private Pathfinder _pathFinder;

        private List<PFPoint> _path = new List<PFPoint>();

        private int _pathIndex;
        private Colony _insectsColony;
        private Vector2 _target;
        public Pheromone ActivePheromone {get; private set;}

        public bool IsWandering { get; private set;}

        public AIHandler(Insect insect, Services services) {
            _insect = insect;
            _ais = new Task[] {
                new Spawn(insect, services.world, this),
                new Collide(insect, services.world, this),
                new DropOff(insect, services.world, this, services.colony),
                new FollowPheromone(insect, services.world, this, services.handler, services.colony),
                new PickUp(insect, services.world, this, services.fruits),
                new Autopilot(insect, services.world, this, services.handler),
            };
            _pathFinder = new Pathfinder(services.world);
            _path = new List<PFPoint>();
            _insectsColony = services.colony;
        }

        public void RunNextTask(GameTime gameTime) {
            foreach (var ai in _ais) {
                if (ai.Run(gameTime)) {
                    return;
                }
            }
        }

        private static Vector2 RotateVector(Vector2 vector, float angle) {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vector2(
                vector.X * cos - vector.Y * sin,
                vector.X * sin + vector.Y * cos
            );
        }

        private bool IsTargetAvailable(Vector2 target) {
            return _insectsColony.TargetPositions.Where(p => Vector2.Distance(p, target) < 24).Count() == 0;
        }

        private Vector2 GetAvailableTarget(Vector2 target) {
            if (IsTargetAvailable(target)) {
                return target;
            }
            int offsetStep = 8;
            var dirFromTarget = (_insect.Position - target).NormalizedCopy(); 
            var closestAvailable = target;
            float closestDist = float.MaxValue;
            float maxDistFromTarget = 96;

            for (int degree = -80; degree <= 80; degree+=10) {
                float angleInRadians = MathHelper.ToRadians(degree); 
                var dir = RotateVector(dirFromTarget, angleInRadians);
                var dist = 0;
                while (dist < maxDistFromTarget) {
                    var newTarget = target + dir * dist;
                    float distToTrueTarget = (target - newTarget).Length();
                    if (IsTargetAvailable(newTarget) && distToTrueTarget < closestDist) {
                        closestAvailable = newTarget;
                        closestDist = distToTrueTarget; 
                    }
                    dist += offsetStep;
                }
            }
            return closestAvailable;
        }

        private void UpdateConolyTargets() {
            if (_path.Count() > 0) {
                _insectsColony.TargetPositions.Add(new Vector2(_path.Last().X, _path.Last().Y));
            }
        }


        public void WalkTo(Vector2 target, Pheromone pheromone, GameTime gameTime, InsectState state) {
            IsWandering = false;
            if (pheromone != ActivePheromone) {
                ActivePheromone?.RemovePathForAnt(_insect.GetHashCode());
                ActivePheromone = pheromone;
            }
            if (Vector2.DistanceSquared(target, _target) > 32) {
                _target = target;

                _path = _pathFinder.FindPath(_insect.Position, GetAvailableTarget(target));
                _pathIndex = 0;
            }
            UpdateConolyTargets();
            if (_pathIndex < _path.Count) {
                var nextPoint = new Vector2(_path[_pathIndex].X, _path[_pathIndex].Y);
                _insect.TargetDirection = nextPoint - _insect.Position; 
                _insect.Walk(gameTime, state);
                if (Vector2.DistanceSquared(nextPoint, _insect.Position) < 256) {
                    _pathIndex++;
                }
                pheromone?.AddPathForAnt(_insect.GetHashCode(), _path.Skip(_pathIndex).ToList());
                return;
            }
            pheromone?.ReachedInsects.Add(_insect);
            pheromone?.RemovePathForAnt(_insect.GetHashCode());
            if (state != InsectState.Fight) {
                Wander(gameTime, state);
            }
        }

        public void Wander(GameTime gameTime, InsectState state) {
            IsWandering = true;
            if (_path.Count > 0) {
                _path = new List<PFPoint>();
                // Don't turn right after finding pheromone
                _nextUpdateTime += 750;
            }
            if (gameTime.TotalGameTime.TotalMilliseconds > _nextUpdateTime) {
                _nextUpdateTime = gameTime.TotalGameTime.TotalMilliseconds + Random.Shared.Next(5000) + 500;
                _insect.TargetRotation = Random.Shared.Next(360);
            }
            if (ActivePheromone != null) {
                Vector2 dir = ActivePheromone.Position - _insect.Position;
                if (dir.Length() > ActivePheromone.Range) {
                    _insect.TargetDirection = dir.NormalizedCopy();
                }
            }
            _insect.Walk(gameTime, state);
        }
    }
}
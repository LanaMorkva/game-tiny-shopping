using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using MonoGame.Extended;
using TinyShopping.Game.Pathfinding;
using PFPoint = TinyShopping.Game.Pathfinding.Point;


namespace TinyShopping.Game.AI {
    internal class AIHandler {
        private float _pheromoneCooldown = 0;
        private float _damageCooldown = 0;
        private double _nextUpdateTime = 0;

        private Insect _insect;
        private Task[] _ais;

        private Pathfinder _pathFinder;

        private List<PFPoint> _path = new List<PFPoint>();

        private int _pathIndex;
        private Vector2 _target;

        public AIHandler(Insect insect, Services services) {
            _insect = insect;
            _ais = new Task[] {
                new Spawn(insect, services.world, this),
                new Collide(insect, services.world, this),
                new DropOff(insect, services.world, this, services.colony),
                new FollowPheromone(insect, services.world, this, services.handler, services.colony),
                new PickUp(insect, services.world, this, services.fruits),
            };
            _pathFinder = new Pathfinder(services.world);
        }

        public void Update(GameTime gameTime) {
            foreach (var ai in _ais) {
                if (ai.Run(gameTime)) {
                    return;
                }
            }
            Wander(gameTime);
        }


        public void WalkTo(Vector2 target, Pheromone pheromone, GameTime gameTime) {
            if (Vector2.DistanceSquared(target, _target) > 32) {
                _target = target;
                _path = _pathFinder.FindPath(_insect.Position, target);
                _pathIndex = 0;
            }
            if (_pathIndex < _path.Count) {
                var nextPoint = new Vector2(_path[_pathIndex].X, _path[_pathIndex].Y);
                _insect.TargetDirection = nextPoint - _insect.Position; 
                var state = _insect.IsCarrying ? Insect.InsectState.CarryRun : Insect.InsectState.Run;
                _insect.Walk(gameTime, state);
                if (Vector2.DistanceSquared(nextPoint, _insect.Position) < 256) {
                    _pathIndex++;
                }
                pheromone?.AddPathForAnt(_insect.GetHashCode(), _path.Skip(_pathIndex).ToList());
                return;
            }
            
            _insect.ActivePheromone = pheromone; 
            pheromone?.RemovePathForAnt(_insect.GetHashCode());
            Wander(gameTime);
        }

        private void Wander(GameTime gameTime) {
            if (_path.Count > 0) {
                _path = new List<PFPoint>();
                // Don't turn right after finding pheromone
                _nextUpdateTime += 750;
            }
            if (gameTime.TotalGameTime.TotalMilliseconds > _nextUpdateTime) {
                _nextUpdateTime = gameTime.TotalGameTime.TotalMilliseconds + Random.Shared.Next(5000) + 500;
                _insect.TargetRotation = Random.Shared.Next(360);
            }
            if (_insect.ActivePheromone != null) {
                Vector2 dir = _insect.ActivePheromone.Position - _insect.Position;
                if (dir.Length() > _insect.ActivePheromone.Range) {
                    _insect.TargetDirection = dir.NormalizedCopy();
                }
            }
            var state = _insect.IsCarrying ? Insect.InsectState.CarryWander : Insect.InsectState.Wander;
            _insect.Walk(gameTime, state);
        }
    }
}
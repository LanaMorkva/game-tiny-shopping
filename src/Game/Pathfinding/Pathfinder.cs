using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace TinyShopping.Game.Pathfinding {

    /// <summary>
    /// The Pathfinder implements A* pathfinding.
    /// </summary>
    internal class Pathfinder {

        private static readonly int RANGE = 25;

        private World _world;

        private ISet<Node> _queue;

        private IDictionary<Point, Node> _nodes;

        private Point _end;

        private IDictionary<Point, Point> _previous;

        public Pathfinder(World world) {
            _world = world;
        }

        /// <summary>
        /// Tries to find a path from start to end.
        /// </summary>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <returns>A list of points leading to the end position.</returns>
        public IList<Point> FindPath(Vector2 start, Vector2 end) {
            _nodes = new Dictionary<Point, Node>(64);
            _queue = new HashSet<Node>(); // TODO: Replace with min heap
            Point startPoint = new Point(start);
            _end = FindViableEndPosition(end);
            Node startNode = new Node(startPoint, 0, startPoint.SquaredDistance(_end));
            _nodes.Add(startPoint, startNode);
            _queue.Add(startNode);
            _previous = new Dictionary<Point, Point>(64);
            int delta = RANGE;
            long minDelta = long.MaxValue;
            Node bestApprox = null;
            while (_queue.Count > 0) {
                Node current = GetNextNode();
                long currentDistance = current.Position.SquaredDistance(_end);
                if (currentDistance <= delta * delta) {
                    IList<Point> path = ConstructPath(current.Position);
                    return path;
                }
                else if (currentDistance < minDelta) {
                    minDelta = currentDistance;
                    bestApprox = current;
                }
                EnqueueNeighbors(current);
            }
            bool targetWalkable = _world.IsWalkable(_end.X, _end.Y, RANGE / 2);
            Debug.Print("Pathfinding finished without result. Best approx distance {0} larger than {1}. Target walkable: {2}", minDelta, delta, targetWalkable);
            return ConstructPath(bestApprox.Position);
        }

        /// <summary>
        /// Finds a viable end position that is walkable.
        /// </summary>
        /// <param name="end">The initial end position, might be unwalkable.</param>
        /// <returns>A position close to the initial position that is walkable.</returns>
        private Point FindViableEndPosition(Vector2 end) {
            int distance = RANGE;
            while (true) {
                for (int dX = -1; dX <= 1; dX++) {
                    for (int dY = -1; dY <= 1; dY++) {
                        if (dX == 0 && dY == 0) {
                            continue;
                        }
                        int x = (int)end.X + dX * distance;
                        int y = (int)end.Y + dY * distance;
                        if (_world.IsWalkable(x, y, RANGE / 2)) {
                            return new Point(x, y);
                        }
                    }
                }
                distance += RANGE;
            }
        }

        /// <summary>
        /// Constructs the final path from the processing results.
        /// </summary>
        /// <param name="current">The final point close to the end.</param>
        /// <returns>The complete path.</returns>
        private IList<Point> ConstructPath(Point current) {
            List<Point> path = new List<Point> {
                _end
            };
            while (_previous.ContainsKey(current)) {
                path.Add(current);
                current = _previous[current];
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Gets the node with the lowest cost + heuristics to use.
        /// </summary>
        /// <returns>The node.</returns>
        private Node GetNextNode() {
            Node next = null;
            long minWeight = long.MaxValue;
            foreach (Node node in _queue) {
                long weight = node.Cost * node.Cost + node.Heuristics;
                if (weight < minWeight) {
                    minWeight = weight;
                    next = node;
                }
            }
            _queue.Remove(next);
            return next;
        }

        /// <summary>
        /// Checks if two points are close enough.
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <returns>If p1 and p2 are close enough.</returns>
        private bool IsCloseEnough(Point p1, Point p2) {
            int delta = RANGE;
            return p1.SquaredDistance(p2) <= delta * delta;
        }

        /// <summary>
        /// Adds the neighbors to the queue if needed.
        /// </summary>
        /// <param name="current">The current node.</param>
        private void EnqueueNeighbors(Node current) {
            int delta = RANGE;
            for (int dX = -1; dX <= 1; dX++) {
                for (int dY = -1; dY <= 1; dY++) {
                    if (dX == 0 && dY == 0) {
                        continue;
                    }
                    Point neighbor = new Point(current.Position.X + delta * dX, current.Position.Y + delta * dY);
                    TryEnqueuePosition(neighbor, current);
                }
            }
        }

        /// <summary>
        /// Adds the given position to the processing queue if the position is walkable.
        /// </summary>
        /// <param name="position">The position to add.</param>
        /// <param name="current">The currently processing position.</param>
        private void TryEnqueuePosition(Point position, Node current) {
            if (!_world.IsWalkable(position.X, position.Y, RANGE / 2)) {
                return;
            }
            int delta = RANGE;
            Node node;
            bool hasNode = _nodes.TryGetValue(position, out node);
            long tentativeCost = current.Cost + delta;
            if (!hasNode || tentativeCost * tentativeCost < node.SquaredCost) {
                Node newNode = new Node(position, tentativeCost, position.SquaredDistance(_end));
                _nodes[position] = newNode;
                _queue.Add(newNode);
                _previous[position] = current.Position;
            }
        }
    }
}

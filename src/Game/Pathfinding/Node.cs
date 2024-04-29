namespace TinyShopping.Game.Pathfinding {

    /// <summary>
    /// A graph vertex for the A* pathfinding.
    /// </summary>
    internal class Node {

        /// <summary>
        /// The corresponding position.
        /// </summary>
        public Point Position { get; private set; }

        /// <summary>
        /// The cost of the path up to this node.
        /// </summary>
        public long Cost { get; private set; }

        /// <summary>
        /// The squared cost of the path up to this node.
        /// </summary>
        public long SquaredCost { get; private set; }

        /// <summary>
        /// The estimated distance to the target node.
        /// </summary>
        public long Heuristics { get; private set; }

        /// <summary>
        /// Creates a new node instance.
        /// </summary>
        /// <param name="position">The corresponding position.</param>
        /// <param name="cost">The cost of the path up to this node.</param>
        /// <param name="heuristics">The estimated distance to the target.</param>
        public Node(Point position, long cost, long heuristics) {
            Position = position;
            Cost = cost;
            SquaredCost = cost * cost;
            Heuristics = heuristics;
        }

        public override int GetHashCode() {
            return Position.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is Node) {
                Node p = (Node)obj;
                return Position.Equals(p.Position);
            }
            return false;
        }
    }
}

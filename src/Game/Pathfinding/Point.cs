using Microsoft.Xna.Framework;

namespace TinyShopping.Game.Pathfinding {

    /// <summary>
    /// A 2D point with int coordinates.
    /// </summary>
    internal class Point {

        /// <summary>
        /// The x coordinate of the point.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The y coordinate of the point.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Creates a new point from two coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Creates a new point from a vector.
        /// </summary>
        /// <param name="vector">The vector to convert to a point.</param>
        public Point(Vector2 vector) {
            X = (int)vector.X;
            Y = (int)vector.Y;
        }

        /// <summary>
        /// Calculates the squared distance between this point and the given point.
        /// </summary>
        /// <param name="point">The target point.</param>
        /// <returns>The squared distance.</returns>
        public long SquaredDistance(Point point) {
            return (X - point.X) * (X - point.X) + (Y - point.Y) * (Y - point.Y);
        }

        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is Point) {
                Point p = (Point)obj;
                return X == p.X && Y == p.Y;
            }
            return false;
        }
    }
}

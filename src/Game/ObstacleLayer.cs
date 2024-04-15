using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Collections.Generic;
using MonoGame.Extended.Tiled;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;

namespace TinyShopping.Game {
    internal class Obstacle {
        public Vector2 Position { get; }
        public Polygon Polygon { get; }

        public Obstacle(Vector2 position, Point2[] points) {
            Position = position;
            List<Vector2> vertices = new List<Vector2>();
            foreach (Point2 point in points) {
                vertices.Add(new Vector2(point.X, point.Y));
            }
            Polygon = new Polygon(vertices);
        }

        public Obstacle(Vector2 position, Size2 size) {
            Position = position;
            List<Vector2> vertices = new List<Vector2>();
            vertices.Add(new Vector2(0, 0));
            vertices.Add(new Vector2(size.Width, 0));
            vertices.Add(new Vector2(size.Width, size.Height));
            vertices.Add(new Vector2(0, size.Height));
            Polygon = new Polygon(vertices);
        }

        public bool Contains(Vector2 point) {
            Vector2 shiftedPoint = point - Position;
            return Polygon.Contains(shiftedPoint);
        }
    }
    internal class ObstacleLayer {
        private List<Obstacle> _obstacles; 


        public ObstacleLayer(TiledMap tiledMap) {
            _obstacles = new List<Obstacle>();
            foreach (TiledMapObject obj in tiledMap.GetLayer<TiledMapObjectLayer>("obstacles").Objects) {
                if (obj is TiledMapPolygonObject polygon) {
                    for (int i = 0; i < polygon.Points.Length; i++) {
                        polygon.Points[i] = Utilities.worldPosToScreen(polygon.Points[i], tiledMap.TileHeight, tiledMap.TileWidth);
                    }
                    var position = Utilities.worldPosToScreen(polygon.Position, tiledMap.TileHeight, tiledMap.TileWidth);
                    _obstacles.Add(new Obstacle(position, polygon.Points));
                } else {
                    //TODO check if it works
                    var position = Utilities.worldPosToScreen(obj.Position, tiledMap.TileHeight, tiledMap.TileWidth);
                    _obstacles.Add(new Obstacle(position, obj.Size));
                }
            }
        }

        /// <summary>
        /// Draws obstacle borders (called only in DEBUG)
        /// </summary>
        /// <param name="batch">SpriteBatch for drawing.</param>
        public void Draw(SpriteBatch batch) {
            foreach (var obstacle in _obstacles) {
                Vector2 position = obstacle.Position;
                Polygon polygon = obstacle.Polygon;
                if (polygon.Vertices.Length > 0) {
                    batch.DrawPolygon(position, polygon, Color.Red, 2f);
                }
            }
        }

        /// <summary>
        /// Checks for collisions with static obstacles
        /// </summary>
        /// <param name="x">Center (X) of the object that is checked</param>
        /// <param name="y">Center (Y) of the object that is checked</param>
        /// <param name="range">Range where collision is happening</param>
        public bool HasCollision(int x, int y, int range) {
            Rectangle objRect = new Rectangle(x - range, y - range, 2*range, 2*range);
            List<Vector2> corners = objRect.GetCorners().Select(point => new Vector2(point.X, point.Y)).ToList();
            foreach (var obstacle in _obstacles) {
                foreach (var corner in corners) {
                    if (obstacle.Contains(corner)) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

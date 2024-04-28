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

        private List<Vector2> _normals;
        private Polygon _polygon;

        public Obstacle(Vector2 position, Point2[] points) {
            List<Vector2> vertices = new List<Vector2>();
            foreach (Point2 point in points) {
                vertices.Add(new Vector2(point.X, point.Y));
            }
            SetParameters(position, vertices);
        }

        public Obstacle(Vector2 position, List<Vector2> vertices) {
            SetParameters(position, vertices);
        }

        public Obstacle(Vector2 position, Size2 size) {
            Vector2[] vertices = {new(0, 0), new(size.Width, 0), new(size.Width, size.Height), new(0, size.Height)};
            SetParameters(position, vertices.ToList());
        }

        private void SetParameters(Vector2 position, List<Vector2> vertices) {
            vertices = vertices.Select(v => v + position).ToList();
            _polygon = new Polygon(vertices);
            _normals = GetNormals(_polygon);
        }

        public bool IsColliding(Polygon otherPoly) {
            List<Vector2> axes = new List<Vector2>();
            axes.AddRange(_normals);
            axes.AddRange(GetNormals(otherPoly));

            foreach (Vector2 axis in axes)  {
                (float minA, float maxA) = Project(_polygon, axis);
                (float minB, float maxB) = Project(otherPoly, axis);

                if (maxA < minB || maxB < minA) {
                    return false;
                }
            }

            return true;
        }

        public void Draw(SpriteBatch batch) {
            batch.DrawPolygon(Vector2.Zero, _polygon, Color.Red, 2f);
        }


        // Helper function to find normals to every edge
        private List<Vector2> GetNormals(Polygon polygon) {
            List<Vector2> normals = new List<Vector2>();
            for (int i = 0; i < polygon.Vertices.Count(); i++) {
                Vector2 edge = polygon.Vertices[(i + 1) % polygon.Vertices.Count()] - polygon.Vertices[i];
                Vector2 normal = new(-edge.Y, edge.X);
                normals.Add(normal.NormalizedCopy()); 
            }
            return normals;
        }

        // Helper function to project the polygon onto a axis
        private (float min, float max) Project(Polygon polygon, Vector2 axis) {
            float min = Vector2.Dot(axis, polygon.Vertices[0]);
            float max = min;
            for (int i = 1; i < polygon.Vertices.Count(); i++) {
                float p = Vector2.Dot(axis, polygon.Vertices[i]);
                if (p < min)
                    min = p;
                else if (p > max)
                    max = p;
            }
            return (min, max);
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
                obstacle.Draw(batch);
            }
        }

        /// <summary>
        /// Checks for collisions with static obstacles
        /// </summary>
        public bool HasCollision(Polygon objPolygon) {
            return _obstacles.Any(o => o.IsColliding(objPolygon));
        }
    }
}

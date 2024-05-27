using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

namespace TinyShopping.Game
{
    public class Camera2D
    {
        public Vector2 Position { get; set; }  = Vector2.Zero;
        public Vector2 TargetMovement {get; set; } = Vector2.Zero;
        public Vector2 Origin { get; private set; }
        public float Zoom { get ; private set; } = 1.0f;

        private Vector2 _size;
        
        public Camera2D(int width, int height) {
            _size = new Vector2(width, height);
            Origin = new Vector2(width / 2f, height / 2f);
        }


        public RectangleF BoundingRectangle () {
            // have to work with 3D vectors, since BoundingFrustrum designed for 3D cases
            Vector3[] corners = GetBoundingFrustum().GetCorners();
            Vector3 topLeft = corners[0];
            Vector3 bottomRight = corners[2];
            float width = bottomRight.X - topLeft.X;
            float height = bottomRight.Y - topLeft.Y;
            return new RectangleF(topLeft.X, topLeft.Y, width, height);
        }

        public void Update () {
            float lerpSpeed = 0.75f;
            if (Math.Abs(TargetMovement.X) > 0 && Math.Abs(TargetMovement.Y) > 0) {
                lerpSpeed = 0.7f;
            }

            Move(Vector2.Lerp(Vector2.Zero, TargetMovement, lerpSpeed)); 
        }
        
        public void Move(Vector2 direction) {
            Position += direction;
        }

        public void LookAt(Vector2 position) {
            Position = position - new Vector2(_size.X / 2f, _size.Y / 2f);
        }

        public void SetZoom(float zoom) {
            Zoom = zoom;
        }

        public void ZoomIn(float deltaZoom) {
            Zoom = MathHelper.Clamp(Zoom + deltaZoom, Constants.ZOOM_MIN, Constants.ZOOM_MAX);
        }

        public void ZoomOut(float deltaZoom) {
            Zoom = MathHelper.Clamp(Zoom - deltaZoom, Constants.ZOOM_MIN, Constants.ZOOM_MAX);
        }

        public Matrix GetViewMatrix() {
            return Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) * Matrix.CreateTranslation(new Vector3(-Origin, 0.0f))*
                Matrix.CreateScale(Zoom, Zoom, 1) * Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        private BoundingFrustum GetBoundingFrustum()
        {
            Matrix viewMatrix = GetViewMatrix();
            Matrix projectionMatrix = GetProjectionMatrix(viewMatrix);
            return new BoundingFrustum(projectionMatrix);
        }

        private Matrix GetProjectionMatrix(Matrix viewMatrix)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, _size.X, _size.Y, 0, -1, 0);
            Matrix.Multiply(ref viewMatrix, ref projection, out projection);
            return projection;
        }

        public ContainmentType Contains(Vector2 vector2) {
            return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0));
        }

        public ContainmentType Contains(Rectangle rectangle) {
            Vector3 max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
            Vector3 min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
            BoundingBox boundingBox = new BoundingBox(min, max);
            return GetBoundingFrustum().Contains(boundingBox);
        }
    }
}
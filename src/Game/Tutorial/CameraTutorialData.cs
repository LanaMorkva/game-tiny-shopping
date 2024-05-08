using System;
using Microsoft.Xna.Framework;


namespace TinyShopping.Game.Tutorial {
    struct CameraTutorialData {
        [Flags]
        private enum CameraMoved {
            Up  = 1,
            Down = 2,
            Left = 4,
            Right = 8,
            ZoomIn = 16,
            ZoomOut = 32,
            Everywhere = Up | Down | Left | Right | ZoomIn | ZoomOut,
        }
        private CameraMoved CameraFlags = 0;
        private float CameraPreviousRecordedZoom = 0.5f;
        private Vector2 CameraPreviousRecordedMovement = Vector2.Zero;
        public CameraTutorialData() {}

        public void Update(Vector2 camUpdateMovement, float camUpdateZoom) {
            if (camUpdateMovement.X > CameraPreviousRecordedMovement.X) {
                CameraFlags |= CameraMoved.Right;
            }
            if (camUpdateMovement.X < CameraPreviousRecordedMovement.X) {
                CameraFlags |= CameraMoved.Left;
            }
            if (camUpdateMovement.Y < CameraPreviousRecordedMovement.Y) {
                CameraFlags |= CameraMoved.Up;
            }
            if (camUpdateMovement.Y > CameraPreviousRecordedMovement.Y) {
                CameraFlags |= CameraMoved.Down;
            }
            if (camUpdateZoom > CameraPreviousRecordedZoom) {
                CameraFlags |= CameraMoved.ZoomIn;
            }
            if (camUpdateZoom < CameraPreviousRecordedZoom) {
                CameraFlags |= CameraMoved.ZoomOut;
            }

            CameraPreviousRecordedMovement = camUpdateMovement;
            CameraPreviousRecordedZoom = camUpdateZoom;
        }
        public bool Completed() {
            return CameraFlags == CameraMoved.Everywhere;
        }
    }
}
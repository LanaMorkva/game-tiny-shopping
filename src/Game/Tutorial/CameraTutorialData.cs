using System;
using Microsoft.Xna.Framework;


namespace TinyShopping.Game.Tutorial {
    struct CameraTutorialData {
        [Flags]
        private enum CameraMoved {
            Horizonal = 1,
            Vertical = 2, 
            Zoomed = 4,
            Everywhere = Horizonal | Vertical | Zoomed,
        }
        private CameraMoved CameraFlags = 0;
        private float CameraPreviousRecordedZoom = 0;
        private Vector2 CameraPreviousRecordedMovement = Vector2.Zero;
        public CameraTutorialData() {}

        public void Update(Vector2 camUpdateMovement, float camUpdateZoom) {
            if (CameraPreviousRecordedMovement == Vector2.Zero && CameraPreviousRecordedZoom == 0) {
                //first recorded movement, record value and skip
                CameraPreviousRecordedMovement = camUpdateMovement;
                CameraPreviousRecordedZoom = camUpdateZoom;
                return;
            }

            if (Math.Abs(camUpdateMovement.X - CameraPreviousRecordedMovement.X) > 1) {
                CameraFlags |= CameraMoved.Horizonal;
            }
            if (Math.Abs(camUpdateMovement.Y - CameraPreviousRecordedMovement.Y) > 1) {
                CameraFlags |= CameraMoved.Vertical;
            }
            if (Math.Abs(camUpdateZoom - CameraPreviousRecordedZoom) > 0f) {
                CameraFlags |= CameraMoved.Zoomed;
            }

            CameraPreviousRecordedMovement = camUpdateMovement;
            CameraPreviousRecordedZoom = camUpdateZoom;
        }
        public bool Completed() {
            return CameraFlags == CameraMoved.Everywhere;
        }
    }
}
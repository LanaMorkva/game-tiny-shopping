using Microsoft.Xna.Framework;
using System;

namespace TinyShopping.Game {

    internal class InsectPos {

        private Vector2 _position;

        private float _rotationRad;

        private float _rotation;

        private Vector2 _direction;

        /// <summary>
        /// The 2D position.
        /// </summary>
        public Vector2 Position {
            get {
                return _position;
            }
        }

        /// <summary>
        /// The x coordinate of the current position.
        /// </summary>
        public int X {
            get {
                return (int)_position.X;
            }
        }

        /// <summary>
        /// The y coordinate of the current position.
        /// </summary>
        public int Y {
            get {
                return (int)_position.Y;
            }
        }

        /// <summary>
        /// The current rotation in radians.
        /// </summary>
        public float Rotation {
            get {
                return _rotationRad;
            }
        }

        private int _targetRotation;

        /// <summary>
        /// The desired rotation of the insect in degrees.
        /// </summary>
        public int TargetRotation {
            get {
                return _targetRotation;
            } 
            set {
                _targetRotation = value;
                if (_targetRotation < 0) {
                    _targetRotation += 360;
                }
                if (_targetRotation > 360) {
                    _targetRotation -= 360;
                }
            }
        }

        public Vector2 TargetDirection {
            set { 
                float angle = MathF.Atan2(value.Y, value.X);
                TargetRotation = (int) MathHelper.ToDegrees(angle) + 90;
            }
        }

        /// <summary>
        /// True if the insect's rotation is equal to the target rotation.
        /// </summary>
        public bool IsTurning {
            get {
                return Math.Abs(TargetRotation - _rotation) > 5;
            }
        }

        /// <summary>
        /// Creates a new object with the given coordinates.
        /// </summary>
        /// <param name="x">The initial x coordinate.</param>
        /// <param name="y">The initial y coordinate.</param>
        /// <param name="rotation">The initial rotation</param>
        public InsectPos(int x, int y, int rotation) {
            _position = new Vector2(x, y);
            _rotation = rotation;
            TargetRotation = rotation;
            Rotate(0);
        }

        /// <summary>
        /// Rotates the insect towards the target rotation.
        /// </summary>
        /// <param name="degrees">The number of degrees to rotate.</param>
        public void Rotate(float degrees) {
            float diff = TargetRotation - _rotation;
            if (Math.Abs(diff) > 180) {
                diff *= -1;
                if (_rotation < TargetRotation) {
                    _rotation += 360;
                }
                else {
                    _rotation -= 360;
                }
            }
            if (diff < degrees || diff == 0) {
                _rotation = TargetRotation;
            }
            else {
                _rotation += diff / Math.Abs(diff) * degrees;
            }
            _rotationRad = MathHelper.ToRadians(_rotation);
            _direction = new Vector2(MathF.Sin(_rotationRad), -MathF.Cos(_rotationRad));
            _direction.Normalize();
        }

        /// <summary>
        /// Moves the insect forward.
        /// </summary>
        /// <param name="steps">The number of pixels to move.</param>
        public void Move(float steps) {
            _position.X += _direction.X * steps;
            _position.Y += _direction.Y * steps;
        }
    }
}

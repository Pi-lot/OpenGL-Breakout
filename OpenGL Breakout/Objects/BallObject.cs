using OpenGL_Breakout.Graphics;
using OpenTK.Mathematics;

namespace OpenGL_Breakout.Objects {
    internal class BallObject : GameObject {
        public float Radius;
        public bool Stuck;
        public bool Sticky, PassThrough;

        public BallObject() : base() {
            Radius = 12.5f;
            Stuck = true;
            Sticky = false;
            PassThrough = false;
        }

        public BallObject(Vector2 pos, float radius, Vector2 velocity, Texture2D sprite)
            : base(pos, new(radius * 2.0f, radius * 2.0f), sprite, Vector3.One, velocity) {
            Radius = radius;
            Stuck = true;
            Sticky = false;
            PassThrough = false;
        }

        public Vector2 Move(float dt, int window_width) {
            if (!Stuck) {
                Position += Velocity * dt;

                if (Position.X <= 0.0f) {
                    Velocity.X = -Velocity.X;
                    Position.X = 0.0f;
                } else if (Position.X + Size.X >= window_width) {
                    Velocity.X = -Velocity.X;
                    Position.X = window_width - Size.X;
                }
                if (Position.Y <= 0.0f) {
                    Velocity.Y = -Velocity.Y;
                    Position.Y = 0.0f;
                }
            }

            return Position;
        }

        public void Reset(Vector2 position, Vector2 velocity) {
            Position = position;
            Velocity = velocity;
            Stuck = true;
            Sticky = false;
            PassThrough = false;
        }
    }
}

using System.Drawing;
using System.Numerics;

namespace OpenGL_Breakout.Structs {
    internal struct Particle {
        public Vector2 Position, Velocity;
        public Vector4 Colour;
        public float Life;

        public Particle() {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Colour = Vector4.One;
            Life = 0;
        }
    }
}

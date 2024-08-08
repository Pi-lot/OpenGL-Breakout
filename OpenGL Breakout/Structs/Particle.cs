using OpenTK.Mathematics;

namespace OpenGL_Breakout.Structs {
    internal struct Particle {
        Vector2 Position, Velocity;
        Vector4 Colour;
        float Life;

        public Particle() {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Colour = Vector4.One;
            Life = 0;
        }
    }
}

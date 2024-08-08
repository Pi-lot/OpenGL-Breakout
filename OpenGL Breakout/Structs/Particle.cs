using OpenTK.Mathematics;

namespace OpenGL_Breakout.Structs {
    internal struct Particle {
        public Vector2 Position, Velocity;
        public Color4 Colour;
        public float Life;

        public Particle() {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Colour = Color4.White;
            Life = 0;
        }
    }
}

using OpenGL_Breakout.Enums;
using OpenTK.Mathematics;

namespace OpenGL_Breakout.Structs {
    internal struct Collision {
        public bool collided;
        public Direction direction;
        public Vector2 Point;
    }
}

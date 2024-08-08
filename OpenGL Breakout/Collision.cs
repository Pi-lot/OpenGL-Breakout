using OpenGL_Breakout.Enums;
using OpenTK.Mathematics;

namespace OpenGL_Breakout {
    internal struct Collision {
        bool collided;
        Direction direction;
        Vector2 Point;
    }
}

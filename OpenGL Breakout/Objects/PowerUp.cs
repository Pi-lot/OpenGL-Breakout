using OpenGL_Breakout.Graphics;
using System.Numerics;

namespace OpenGL_Breakout.Objects {
    internal class PowerUp : GameObject {
        static Vector2 POWERUP_SIZE = new(60.0f, 20.0f);
        static Vector2 VELOCITY = new(0.0f, 150.0f);

        public string Type;
        public float Duration;
        public bool Activated;

        public PowerUp(string type, Vector3 colour, float duration, Vector2 position, Texture2D texture) 
            : base(position, POWERUP_SIZE, texture, colour, VELOCITY) {
            Type = type;
            Duration = duration;
            Activated = false;
        }
    }
}

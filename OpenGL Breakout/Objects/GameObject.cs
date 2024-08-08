using OpenGL_Breakout.Graphics;
using OpenTK.Mathematics;

namespace OpenGL_Breakout.Objects {
    internal class GameObject {
        public Vector2 Position, Size, Velocity;
        public Vector3 Colour;
        public float Rotation;
        public bool IsSolid;
        public bool Destroyed;
        public Texture2D Sprite;
        
        public GameObject() {
            Position = Vector2.Zero;
            Size = Vector2.One;
            Velocity = Vector2.Zero;
            Colour = Vector3.One;
            Rotation = 0;
            Sprite = new();
            IsSolid = false;
            Destroyed = false;
        }

        public GameObject(Vector2 pos, Vector2 size, Texture2D sprite, Vector3 colour = new(), Vector2 velocity = new()) {
            Position = pos;
            Size = size;
            Velocity = velocity;
            Colour = colour;
            Rotation = 0;
            Sprite = sprite;
            IsSolid = false;
            Destroyed = false;
        }

        public virtual void Draw(SpriteRenderer renderer) {
            renderer.DrawSprite(Sprite, Position, Size, Rotation, Colour);
        }
    }
}

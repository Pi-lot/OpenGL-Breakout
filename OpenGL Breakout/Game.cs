using OpenGL_Breakout.Enums;
using OpenGL_Breakout.Graphics;
using OpenGL_Breakout.Resources;
using OpenGL_Breakout.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using OpenGL_Breakout.Structs;

namespace OpenGL_Breakout {
    internal class Game {
        public GameState State { get; private set; }
        public bool[] Keys { get; set; } = new bool[1024];

        public int Width, Height;

        public List<GameLevel> Levels;
        public List<PowerUp> PowerUps;
        public int Level;

        Vector2 PLAYER_SIZE = new(100.0f, 20.0f);
        float PLAYER_VELOCITY = 500.0f;

        Vector2 INITIAL_BALL_VELOCITY = new(100.0f, -350.0f);
        float BALL_RADIUS = 12.65f;

        Collision collision;

        SpriteRenderer spriteRenderer;

        private void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam) {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine("OpenGL Debug: {0} | {1}", msg, type);

            if (type == DebugType.DebugTypeError) {
                ErrorCode error = GL.GetError();
                if (error != ErrorCode.NoError) {
                    Console.WriteLine("OpenGL Error: {0}", error);
                    switch (error) {
                        case ErrorCode.InvalidOperation:
                            throw new InvalidOperationException(msg);
                        case ErrorCode.OutOfMemory:
                            throw new OutOfMemoryException(msg);
                        case ErrorCode.InvalidValue:
                            throw new ArgumentException(msg);
                        default:
                            throw new Exception(msg);
                    }
                }
            }
        }

        public Game(int width, int height) {
            State = GameState.GAME_ACTIVE;
            Width = width;
            Height = height;

            GL.Enable(EnableCap.DebugOutput);
            GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        }

        public void Init() {
            ResourceManager.LoadShader("Shaders/sprite.vert", "Shaders/sprite.frag", null, "sprite");

            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f, Width, Height, 0.0f, -1.0f, 1.0f);
            ResourceManager.GetShader("sprite").Use();
            ResourceManager.LoadTexture("Textures/awesomeface.png", true, "face");
            ResourceManager.GetShader("sprite").SetInteger("image", 0);
            ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);
            spriteRenderer = new(ResourceManager.GetShader("sprite"));
        }

        public void ProcessInput(float dt) {
            throw new NotImplementedException();

        }

        public void Update(float dt) {
            throw new NotImplementedException();

        }

        public void Render() {
            spriteRenderer.DrawSprite(ResourceManager.GetTexture("face"), new Vector2(200.0f, 200.0f), new Vector2(300.0f, 400.0f), 45.0f, new Vector3(0.0f, 1.0f, 0.0f));
        }

        public void DoCollisions() {
            throw new NotImplementedException();

        }

        public void ResetLevel() {
            throw new NotImplementedException();

        }

        public void ResetPlayer() {
            throw new NotImplementedException();

        }

        public void SpawnPowerUps(GameObject block) {
            throw new NotImplementedException();

        }

        public void UpdatePowerUps(float dt) {
            throw new NotImplementedException();

        }

        public void Close(CancelEventArgs e) {
            ResourceManager.Clear();

            spriteRenderer.Dispose();
        }
    }
}
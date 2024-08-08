using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using OpenGL_Breakout.Resources;

using OpenGL_Breakout.Resources;

namespace OpenGL_Breakout {
    internal class GLWindow : GameWindow {
        Game breakout;

        float moveSpeed = 20.0f;
        float horOff = 0.0f;
        float verOff = 0.0f;

        bool closing = false;

        public GLWindow(int width, int height, string title) :
            base(GameWindowSettings.Default, new NativeWindowSettings() {
                Size = (width, height),
                Title = title,
                Vsync = VSyncMode.Off
            }) {
            breakout = new(Size.X, Size.Y);
        }

        protected override void OnLoad() {
            base.OnLoad();

            GL.Viewport(0,0, Size.X, Size.Y);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            breakout.Init();
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);

            breakout.ProcessInput((float)args.Time);

            if (KeyboardState.IsKeyDown(Keys.Q)) {
                horOff -= moveSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.E)) {
                horOff += moveSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.W)) {
                verOff -= moveSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.S)) {
                verOff += moveSpeed * (float)args.Time;
            }

            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0.0f + horOff, (float)Size.X + horOff, (float)Size.Y + verOff, 0.0f + verOff, -1.0f, 1.0f);

            try {
                ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);
                ResourceManager.GetShader("particle").SetMatrix4("projection", projection);
            } catch {

            }

            breakout.Update((float)args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);
            
            if (closing)
                return;

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            breakout.Render((float)args.Time);

            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Keys.Escape)
                Close();
            if ((int)e.Key >= 0 && (int)e.Key < 1024) {
                breakout.keys[(int)e.Key] = true;
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);

            if ((int)e.Key >= 0 && (int)e.Key < 1024) {
                breakout.keys[(int)e.Key] = false;
            }
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);

            breakout.Width = Size.X;
            breakout.Height = Size.Y;

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            closing = true;
            breakout.Close(e);
        }
    }
}

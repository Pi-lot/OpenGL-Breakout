using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
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
                ClientSize = (width, height),
                Title = title,
                Vsync = VSyncMode.Off
            }) {
            breakout = new(ClientSize.X, ClientSize.Y);
        }

        protected override void OnLoad() {
            base.OnLoad();

            GL.Viewport(0,0, ClientSize.X, ClientSize.Y);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            breakout.Init();
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);

            breakout.ProcessInput((float)args.Time);

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

            breakout.Width = ClientSize.X;
            breakout.Height = ClientSize.Y;

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            closing = true;
            breakout.Close(e);
        }
    }
}

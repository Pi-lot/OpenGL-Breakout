using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;

namespace OpenGL_Breakout {
    internal class GLWindow : GameWindow {
        Game breakout;

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
            breakout.Init();
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);

            breakout.ProcessInput((float)args.Time);

            breakout.Update((float)args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            breakout.Render();

            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.Key == Keys.Escape)
                Close();
            if ((int)e.Key >= 0 && (int)e.Key < 1024) {
                breakout.Keys[(int)e.Key] = true;
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);

            if ((int)e.Key >= 0 && (int)e.Key < 1024) {
                breakout.Keys[(int)e.Key] = false;
            }
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            ResourceManager.Clear();
        }
    }
}

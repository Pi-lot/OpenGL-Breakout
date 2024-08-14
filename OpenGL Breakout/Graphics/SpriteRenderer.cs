using System.Numerics;
using OpenTK.Graphics.OpenGL4;
using OpenGL_Breakout.Resources;

namespace OpenGL_Breakout.Graphics {
    internal class SpriteRenderer : IDisposable {
        private bool disposedValue;
        private Shader shader;
        private int quadVAO;

        public SpriteRenderer(Shader shader) {
            this.shader = shader;
            initRenderData();
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, float rotate, Vector3 colour) {
            shader.Use();
            Matrix4x4 model = Matrix4x4.Identity;
            model = Matrix4x4.CreateTranslation(new Vector3(position, 0.0f)) * model;
            
            model = Matrix4x4.CreateTranslation(new Vector3(0.5f * size.X, 0.5f * size.Y, 0.0f)) * model;
            model = Matrix4x4.CreateRotationZ(rotate) * model;
            model = Matrix4x4.CreateTranslation(new Vector3(-0.5f * size.X, -0.5f * size.Y, 0.0f)) * model;

            model = Matrix4x4.CreateScale(new Vector3(size, 0.0f)) * model;

            shader.SetMatrix4x4("model", model);
            shader.SetVector3("spriteColour", colour);

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();

            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        private void initRenderData() {
            int VBO;
            float[] vertices = { 
                // pos      // tex
                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,

                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f
            };

            quadVAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(quadVAO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                try {
                    GL.DeleteVertexArray(quadVAO);
                } catch (Exception e) {
                    Console.WriteLine("Error Deleting Sprite Renderer VAO: " + e.Message);
                }

                //try {
                //    GL.DeleteVertexArray(VBO);
                //} catch (Exception e) {
                //    Console.WriteLine("Error Deleting Sprite Renderer VBO: " + e.Message);
                //}

                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

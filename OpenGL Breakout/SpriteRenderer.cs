using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Breakout {
    internal class SpriteRenderer : IDisposable {
        private bool disposedValue;
        private Shader shader;
        private int quadVAO, VBO;

        public SpriteRenderer(Shader shader) {
            this.shader = shader;
            initRenderData();
        }

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, float rotate, Vector3 colour) {
            shader.Use();
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(new Vector3(position));

            model *= Matrix4.CreateTranslation(new Vector3(0.5f * size.X, 0.5f * size.Y, 0.0f));
            model *= Matrix4.CreateRotationZ(rotate);
            model *= Matrix4.CreateTranslation(new Vector3(-0.5f * size.X, -0.5f * size.Y, 0.0f));

            model *= Matrix4.CreateScale(new Vector3(size));

            shader.SetMatrix4("model", model);
            shader.SetVector3("spriteColour", colour);

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();

            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            //GL.BindVertexArray(0);
        }

        private void initRenderData() {
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
            //GL.BindVertexArray(0);
        }

        public virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                try {
                    GL.DeleteVertexArray(quadVAO);
                } catch (Exception e) {
                    Console.WriteLine("Error Deleting Sprite Renderer VAO: " + e.Message);
                }

                try {
                    GL.DeleteVertexArray(VBO);
                } catch (Exception e) {
                    Console.WriteLine("Error Deleting Sprite Renderer VBO: " + e.Message);
                }

                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

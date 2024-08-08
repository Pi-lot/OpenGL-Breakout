using OpenTK.Graphics.OpenGL4;

namespace OpenGL_Breakout.Graphics {
    internal class Texture2D : IDisposable {
        private bool disposedValue = false;
        public int ID { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public PixelInternalFormat Internal_Format { get; set; }
        public PixelFormat Image_Format { get; set; }
        public TextureWrapMode Wrap_S { get; set; }
        public TextureWrapMode Wrap_T { get; set; }
        public TextureMinFilter Filter_Min { get; set; }
        public TextureMagFilter Filter_Max { get; set; }

        public Texture2D() {
            Width = 0;
            Height = 0;
            Internal_Format = PixelInternalFormat.Rgb;
            Image_Format = PixelFormat.Rgb;
            Wrap_S = TextureWrapMode.Repeat;
            Wrap_T = TextureWrapMode.Repeat;
            Filter_Min = TextureMinFilter.Linear;
            Filter_Max = TextureMagFilter.Linear;

            ID = GL.GenTexture();
        }

        public void Generate(int width, int height, byte[] data) {
            Width = width;
            Height = height;

            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, Internal_Format, Width, Height, 0, Image_Format, PixelType.UnsignedByte, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Filter_Min);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Filter_Max);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)Wrap_S);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)Wrap_T);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind() {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                try {
                    GL.DeleteTexture(ID);
                } catch (Exception e) {
                    Console.WriteLine("Error Deleting Texture: " + e.Message);
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

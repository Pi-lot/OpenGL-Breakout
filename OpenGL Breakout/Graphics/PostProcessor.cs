using OpenGL_Breakout.Resources;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_Breakout.Graphics {
    internal class PostProcessor {
        public Shader PostProcessingShader;
        public Texture2D Texture;
        public int Width, Height;
        public bool Confuse, Chaos, Shake;

        private int MSFBO, FBO;
        private int RBO;
        private int VAO;

        private void InitRenderData() {
            int VBO;

            float[] verticies = [
		        // pos        // tex
		        -1.0f, -1.0f, 0.0f, 0.0f,
                1.0f,  1.0f, 1.0f, 1.0f,
                -1.0f,  1.0f, 0.0f, 1.0f,

                -1.0f, -1.0f, 0.0f, 0.0f,
                1.0f, -1.0f, 1.0f, 0.0f,
                1.0f,  1.0f, 1.0f, 1.0f
            ];

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verticies.Length, verticies, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public PostProcessor(Shader shader, int width, int height) {
            PostProcessingShader = shader;
            Texture = new();
            Width = width;
            Height = height;
            Confuse = false;
            Chaos = false;
            Shake = false;

            MSFBO = GL.GenFramebuffer();
            FBO = GL.GenFramebuffer();
            RBO = GL.GenRenderbuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, MSFBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBO);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 4, RenderbufferStorage.Rgb4, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, RBO);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("Post Processor: Failed to initialize MSFBO");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            Texture.Generate(width, height, null);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture.ID, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("Post Processor: Failed to initialize FBO");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            InitRenderData();
            PostProcessingShader.SetInteger("scene", 0, true);
            float offset = 1.0f / 300.0f;
            float[,] offsets = {
                { -offset,  offset  },  // top-left
		        {  0.0f,    offset  },  // top-center
		        {  offset,  offset  },  // top-right
		        { -offset,  0.0f    },  // center-left
		        {  0.0f,    0.0f    },  // center-center
		        {  offset,  0.0f    },  // center - right
		        { -offset, -offset  },  // bottom-left
		        {  0.0f,   -offset  },  // bottom-center
		        {  offset, -offset  }   // bottom-right    
	        };
            GL.Uniform2(GL.GetUniformLocation(PostProcessingShader.ID, "offsets"), 9, offsets.Cast<float>().ToArray());

            int[] edge_kernel = [
                -1, -1, -1,
                -1,  8, -1,
                -1, -1, -1
            ];
            GL.Uniform2(GL.GetUniformLocation(PostProcessingShader.ID, "edge_kernel"), 9, edge_kernel);

            float[] blur_kernel = [
                1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f,
                2.0f / 16.0f, 4.0f / 16.0f, 2.0f / 16.0f,
                1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f
            ];
            GL.Uniform2(GL.GetUniformLocation(PostProcessingShader.ID, "blur_kernel"), 9, blur_kernel);
        }

        public void BeginRender() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, MSFBO);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void EndRender() {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, MSFBO);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, FBO);
            GL.BlitFramebuffer(0, 0, Width, Height, 0, 0, Width, Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render(float time) {
            PostProcessingShader.Use();
            PostProcessingShader.SetFloat("time", time);
            int confuse;
            if (Confuse)
                confuse = 1;
            else 
                confuse = 0;
            PostProcessingShader.SetInteger("confuse", confuse);
            int chaos;
            if (Chaos)
                chaos = 1;
            else 
                chaos = 0;
            PostProcessingShader.SetInteger("chaos", chaos);
            int shake;
            if (Shake)
                shake = 1;
            else
                shake = 0;
            PostProcessingShader.SetInteger("shake", shake);

            GL.ActiveTexture(TextureUnit.Texture0);
            Texture.Bind();
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
    }
}

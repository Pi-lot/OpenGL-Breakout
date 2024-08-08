using OpenGL_Breakout.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL_Breakout.Graphics {
    internal class PostProcessor {
        public Shader PostProcessingShader;
        public Texture2D texture;
        public int Width, Height;
        public bool Confuse, Chaos, Shake;

        private int MSFBO, FBO;
        private int RBO;
        private int VAO;

        private void InitRenderData() {
            throw new NotImplementedException();

        }

        public PostProcessor(Shader shader, int width, int height) { 
            throw new NotImplementedException();
        }

        public void BeginRender() {
            throw new NotImplementedException();

        }

        public void EndRender() { 
            throw new NotImplementedException();
        }

        public void Render() {
            throw new NotImplementedException();
        }
    }
}

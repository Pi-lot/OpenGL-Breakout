using OpenGL_Breakout.Objects;
using OpenGL_Breakout.Resources;
using OpenGL_Breakout.Structs;
using System.Numerics;

namespace OpenGL_Breakout.Graphics {
    internal class ParticleGenerator {
        private int noParticles;
        private List<Particle> particles;
        private Shader shader;
        private Texture2D texture;
        private int VAO;

        private void Init() {
            throw new NotImplementedException();
        }

        private int FirstUnusedParticle() {
            throw new NotImplementedException();

        }

        private void RespawnParticle(Particle particle, GameObject gameObject, Vector2 offset = new()) {
            throw new NotImplementedException();

        }

        public ParticleGenerator(Shader shader, Texture2D texture, int amount) {
            throw new NotImplementedException();

        }

        public void Update(float dt, GameObject gameObject, int noNewParticles, Vector2 offset = new()) {
            throw new NotImplementedException();

        }

        public void Draw() {
            throw new NotImplementedException();
        }
    }
}

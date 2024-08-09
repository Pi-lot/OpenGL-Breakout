using OpenGL_Breakout.Objects;
using OpenGL_Breakout.Resources;
using OpenGL_Breakout.Structs;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace OpenGL_Breakout.Graphics {
    internal class ParticleGenerator {
        private int noParticles;
        private List<Particle> particles = new();
        private Shader shader;
        private Texture2D texture;
        private int VAO;

        private int lastUsedParticle = 0;

        private void Init() {
            int VBO;
            float[] particle_quad = {
                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 0.0f,

                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 0.0f
            };
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * particle_quad.Length, particle_quad, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindVertexArray(0);

            for (int i = 0; i < noParticles; i++)
                particles.Add(new Particle());
        }

        private int FirstUnusedParticle() {
            for (int i = lastUsedParticle; i < noParticles; i++) {
                if (particles[i].Life <= 0.0f) {
                    lastUsedParticle = i;
                    return i;
                }
            }

            for (int i = 0; i < lastUsedParticle; i++) {
                if (particles[i].Life <= 0.0f) {
                    lastUsedParticle = i;
                    return i;
                }
            }

            lastUsedParticle = 0;
            return 0;
        }

        private void RespawnParticle(Particle particle, GameObject gameObject, Vector2 offset = new()) {
            Random r = new Random();
            float random = (r.Next(100) - 50) / 10.0f;
            float rColour = 0.5f + (r.Next(100) / 100.0f);
            particle.Position = gameObject.Position + new Vector2(random) + offset;
            particle.Colour = new(rColour, rColour, rColour, 1.0f);
            particle.Life = 1.0f;
            particle.Velocity = gameObject.Velocity * 0.1f;
        }

        public ParticleGenerator(Shader shader, Texture2D texture, int amount) {
            this.shader = shader;
            this.texture = texture;
            noParticles = amount;

            Init();
        }

        public void Update(float dt, GameObject gameObject, int noNewParticles, Vector2 offset = new()) {
            for (int i = 0; i < noNewParticles; i++) {
                int unusedParticle = FirstUnusedParticle();
                RespawnParticle(particles[unusedParticle], gameObject, offset);
            }

            for (int i = 0; i < noParticles; i++) {
                Particle p = particles[i];
                p.Life -= dt;
                if (p.Life <= 0.0f) {
                    p.Position -= p.Velocity * dt;
                    p.Colour.W -= dt * 2.5f;
                }
            }
        }

        public void Draw() {
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            shader.Use();
            foreach (Particle particle in particles) {
                if (particle.Life > 0.0f) {
                    shader.SetVector2("offset", particle.Position);
                    shader.SetVector4("colour", particle.Colour);
                    texture.Bind();

                    GL.BindVertexArray(VAO);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                    GL.BindVertexArray(0);
                }
            }

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
    }
}

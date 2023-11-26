using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Breakout {
    internal class Shader {
        public int ID { get; private set; }

        public Shader() {

        }

        public void Use() {
            GL.UseProgram(ID);
        }

        public void Compile(string vertexSource, string fragmentSource, string? geometrySource = null) {
            int sVertex, sFragment;
            int? gShader = null;

            sVertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(sVertex, vertexSource);
            GL.CompileShader(sVertex);
            CheckCompileErrors(sVertex, "VERTEX");

            sFragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(sFragment, fragmentSource);
            GL.CompileShader(sFragment);
            CheckCompileErrors(sFragment, "FRAGMENT");

            if (geometrySource != null) {
                gShader = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource((int)gShader, geometrySource);
                GL.CompileShader((int)gShader);
                CheckCompileErrors((int)gShader, "GEOMETRY");
            }

            ID = GL.CreateProgram();
            GL.AttachShader(ID, sVertex);
            GL.AttachShader(ID, sFragment);
            if (geometrySource != null)
                GL.AttachShader(ID, (int)gShader);
            GL.LinkProgram(ID);
            CheckCompileErrors(ID, "PROGRAM");
            GL.DeleteShader(sVertex);
            GL.DeleteShader(sFragment);
            if (geometrySource != null)
                GL.DeleteShader((int)gShader);

        }

        private void CheckCompileErrors(int obj, string type) {
            int success;
            string infoLog;
            if (type == "PROGRAM") {
                GL.GetShaderInfoLog(obj, out infoLog);
                Console.WriteLine("ERROR: Compile-time error: Type: {0}", type);
                Console.WriteLine("{0}", infoLog);
                Console.WriteLine("-----------------------------------------------------------------------");
            } else {
                GL.GetProgramInfoLog(obj, out infoLog);
                Console.WriteLine("ERROR: Link-time error: Type: {0}", type);
                Console.WriteLine("{0}", infoLog);
                Console.WriteLine("-----------------------------------------------------------------------");
            }
        }

        public void SetFloat(string name, float value, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform1(GL.GetUniformLocation(ID, name), value);
        }

        public void SetInteger(string name, float value, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform1(GL.GetUniformLocation(ID, name), value);
        }

        public void SetVector2f(string name, float x, float y, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform2(GL.GetUniformLocation(ID, name), x, y);
        }

        public void SetVector2(string name, Vector2 value, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform2(GL.GetUniformLocation(ID, name), value);
        }

        public void SetVector3(string name, float x, float y, float z, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform3(GL.GetUniformLocation(ID, name), x, y, z);
        }

        public void SetVector3(string name, Vector3 value, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform3(GL.GetUniformLocation(ID, name), value);
        }

        public void SetVector4(string name, float x, float y, float z, float w, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform4(GL.GetUniformLocation(ID, name), x, y, z, w);
        }

        public void SetVector4(string name, Vector4 value, bool useShader = false) {
            if (useShader)
                Use();
            GL.Uniform4(GL.GetUniformLocation(ID, name), value);
        }

        public void SetMatrix4(string name, Matrix4 value, bool useShader = false) {
            if (useShader)
                Use();
            GL.UniformMatrix4(GL.GetUniformLocation(ID, name), false, ref value);
        }
    }
}

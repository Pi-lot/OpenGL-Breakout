using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL_Breakout.Resources {
    internal class Shader : IDisposable {
        private bool disposedValue = false;

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
            try {
                GL.CompileShader(sVertex);
            } catch (InvalidOperationException ioe) {
                Console.WriteLine(ioe.Message);
                CheckCompileErrors(sVertex, "VERTEX");
            }

            sFragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(sFragment, fragmentSource);
            try {
                GL.CompileShader(sFragment);
            } catch (InvalidOperationException ioe) {
                Console.WriteLine(ioe.Message);
                CheckCompileErrors(sFragment, "FRAGMENT");
            }

            if (geometrySource != null) {
                gShader = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource((int)gShader, geometrySource);
                GL.CompileShader((int)gShader);
                CheckCompileErrors((int)gShader, "GEOMETRY");
            }

            ID = GL.CreateProgram();
            try {
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
            } catch (InvalidOperationException ioe) {
                Console.WriteLine(ioe.Message);
            }
        }

        private void CheckCompileErrors(int obj, string type) {
            int success;
            string infoLog;

            if (type != "PROGRAM") {
                GL.GetShader(ID, ShaderParameter.CompileStatus, out success);
                if (success == 0) {
                    infoLog = GL.GetShaderInfoLog(obj);
                    Console.WriteLine("ERROR: Compile-time error: Type: {0} Handle: {1}", type, obj);
                    Console.WriteLine("{0}", infoLog);
                    Console.WriteLine("-----------------------------------------------------------------------");

                    ErrorCode error = GL.GetError();
                    if (error == ErrorCode.InvalidValue || error == ErrorCode.InvalidOperation) {
                        throw new InvalidOperationException(infoLog);
                    }
                }
            } else {
                GL.GetProgram(ID, GetProgramParameterName.LinkStatus, out success);
                if (success == 0) {
                    infoLog = GL.GetProgramInfoLog(obj);
                    Console.WriteLine("ERROR: Link-time error: Type: {0}", type);
                    Console.WriteLine("{0}", infoLog);
                    Console.WriteLine("-----------------------------------------------------------------------");

                    ErrorCode error = GL.GetError();
                    if (error == ErrorCode.InvalidValue || error == ErrorCode.InvalidOperation) {
                        throw new InvalidOperationException(infoLog);
                    }
                }
            }
        }

        public void SetFloat(string name, float value, bool useShader = false) {
            if (useShader)
                Use();
            try {
                GL.Uniform1(GL.GetUniformLocation(ID, name), value);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Float: " + e.Message);
            }
        }

        public void SetInteger(string name, int value, bool useShader = false) {
            if (useShader)
                Use();
            try {
                GL.Uniform1(GL.GetUniformLocation(ID, name), value);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Integer: " + e.Message);
            }
        }

        public void SetVector2f(string name, float x, float y, bool useShader = false) {
            if (useShader)
                Use();
            try {
                GL.Uniform2(GL.GetUniformLocation(ID, name), x, y);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Vector2(x,y): " + e.Message);
            }
        }

        public void SetVector2(string name, Vector2 value, bool useShader = false) {
            float[] val = new float[2];
            value.TryCopyTo(val);
            if (useShader)
                Use();
            try {
                GL.Uniform2(GL.GetUniformLocation(ID, name), 1, val);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Vector2(Vector2): " + e.Message);
            }
        }

        public void SetVector3(string name, float x, float y, float z, bool useShader = false) {
            if (useShader)
                Use();
            try {
                GL.Uniform3(GL.GetUniformLocation(ID, name), x, y, z);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Vector3(x,y,z): " + e.Message);
            }
        }

        public void SetVector3(string name, Vector3 value, bool useShader = false) {
            float[] val = new float[3];
            value.TryCopyTo(val);
            if (useShader)
                Use();
            try {
                GL.Uniform3(GL.GetUniformLocation(ID, name), 1, val);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Vector3(Vector3): " + e.Message);
            }
        }

        public void SetVector4(string name, float x, float y, float z, float w, bool useShader = false) {
            if (useShader)
                Use();
            try {
                GL.Uniform4(GL.GetUniformLocation(ID, name), x, y, z, w);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Vector4(x,y,z,w): " + e.Message);
            }
        }

        public void SetVector4(string name, Vector4 value, bool useShader = false) {
            float[] val = new float[4];
            var success = value.TryCopyTo(val);
            if (useShader)
                Use();
            try {
                GL.Uniform4(GL.GetUniformLocation(ID, name), 1, val);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Vector4(Vector4): " + e.Message);
            }
        }

        public void SetMatrix4x4(string name, Matrix4x4 value, bool useShader = false) {
            float[] val = new float[4 * 4];
            for (int i = 0; i < 4 * 4; i += 4)
                for (int j = 0; j < 4; j++)
                    val[i + j] = value[i / 4, j];
            if (useShader)
                Use();
            try {
                GL.UniformMatrix4(GL.GetUniformLocation(ID, name), 1, false, val);
            } catch (Exception e) {
                Console.WriteLine("Error Setting Matrix4x4: " + e.Message);
            }
        }

        public virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                try {
                    GL.DeleteProgram(ID);
                } catch (Exception e) {
                    Console.WriteLine("Error Deleting Shader Program: " + e.Message);
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

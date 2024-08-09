using OpenGL_Breakout.Graphics;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OpenGL_Breakout.Resources {
    internal static class ResourceManager {
        public static Dictionary<string, Shader> Shaders = new();
        public static Dictionary<string, Texture2D> Textures = new();

        public static Shader LoadShader(string vShaderFile, string fShaderFile, string? gShaderFile, string name) {
            if (Shaders.ContainsKey(name))
                Shaders[name] = LoadShaderFromFile(vShaderFile, fShaderFile, gShaderFile);
            else
                Shaders.Add(name, LoadShaderFromFile(vShaderFile, fShaderFile, gShaderFile));
            return Shaders[name];
        }

        public static Shader GetShader(string name) {
            return Shaders[name];
        }

        public static Texture2D LoadTexture(string file, bool alpha, string name) {
            if (Textures.ContainsKey(name))
                Textures[name] = LoadTextureFromFile(file, alpha);
            else
                Textures.Add(name, LoadTextureFromFile(file, alpha));
            return Textures[name];
        }

        public static Texture2D GetTexture(string name) {
            return Textures[name];
        }

        public static void Clear() {
            foreach (var shader in Shaders.Values)
                shader.Dispose();
            foreach (var texture in Textures.Values)
                texture.Dispose();
            Shaders.Clear();
            Textures.Clear();
        }

        private static Shader LoadShaderFromFile(string vShaderFile, string fShaderFile, string? gShaderFile = null) {
            string vertexCode = string.Empty, fragmentCode = string.Empty;
            string? geometryCode = null;

            try {
                vertexCode = File.ReadAllText(vShaderFile);
                fragmentCode = File.ReadAllText(fShaderFile);
                if (gShaderFile != null)
                    geometryCode = File.ReadAllText(gShaderFile);
            } catch {
                Console.WriteLine("ERROR: Failed to read shader files");
            }

            Shader shader = new();
            shader.Compile(vertexCode, fragmentCode, geometryCode);
            return shader;
        }

        private static Texture2D LoadTextureFromFile(string file, bool alpha) {
            Texture2D texture = new();
            ColorComponents colourComponents;

            if (alpha) {
                texture.Image_Format = PixelFormat.Rgba;
                texture.Internal_Format = PixelInternalFormat.Rgba;
                colourComponents = ColorComponents.RedGreenBlueAlpha;
            } else
                colourComponents = ColorComponents.RedGreenBlue;

            //StbImage.stbi_set_flip_vertically_on_load(1);

            if (!File.Exists(file))
                throw new FileNotFoundException("File doesn't exist");

            using FileStream fileStream = File.OpenRead(file);
            ImageResult image = ImageResult.FromStream(fileStream, colourComponents);
            texture.Generate(image.Width, image.Height, image.Data);

            return texture;
        }
    }
}

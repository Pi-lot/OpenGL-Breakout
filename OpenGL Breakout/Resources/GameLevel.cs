using OpenGL_Breakout.Graphics;
using OpenGL_Breakout.Objects;
using OpenTK.Mathematics;

namespace OpenGL_Breakout.Resources {
    internal class GameLevel {
        public List<GameObject> Bricks = new();

        private void Init(List<List<int>> tileData, int levelWidth, int levelHeight) {
            int height = tileData.Count;
            int width = tileData[0].Count;
            float unit_width = levelWidth / (float)width;
            float unit_height = levelHeight / (float)height;
            
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (tileData[y][x] == 1) {
                        Vector2 pos = new Vector2(unit_width * x, unit_height * y);
                        Vector2 size = new Vector2(unit_width, unit_height);
                        GameObject obj = new GameObject(pos, size, ResourceManager.GetTexture("block_solid"), new Vector3(0.8f, 0.8f, 0.7f));
                        obj.IsSolid = true;
                        Bricks.Add(obj);
                    } else if (tileData[y][x] > 1) {
                        Vector3 colour = Vector3.One;
                        if (tileData[y][x] == 2)
                            colour = new Vector3(0.2f, 0.6f, 1.0f);
                        else if (tileData[y][x] == 3)
                            colour = new Vector3(0.0f, 0.7f, 0.0f);
                        else if (tileData[y][x] == 4)
                            colour = new Vector3(0.8f, 0.8f, 0.4f);
                        else if (tileData[y][x] == 5)
                            colour = new Vector3(1.0f, 0.5f, 0.0f);

                        Vector2 pos = new(unit_width * x, unit_height * y);
                        Vector2 size = new(unit_width, unit_height);
                        Bricks.Add(new GameObject(pos, size, ResourceManager.GetTexture("block"), colour));
                    }
                }
            }
        }

        public void Load(string file, int levelWidth, int levelHeight) {
            Bricks.Clear();
            if (File.Exists(file)) {
                string[] lines = File.ReadAllLines(file);
                List<List<int>> tileData = new();
                foreach (string line in lines) {
                    List<int> row = new();
                    foreach (var s in line.Split(" "))
                        try {
                            row.Add(int.Parse(s));
                        } catch {

                        }
                    tileData.Add(row);
                }
                if (tileData.Count > 0)
                    Init(tileData, levelWidth, levelHeight);
            }
        }

        public void Draw(SpriteRenderer renderer) {
            foreach (var tile in Bricks)
                if (!tile.Destroyed)
                    tile.Draw(renderer);
        }

        public bool IsCompleted() {
            foreach (var tile in Bricks)
                if (!tile.IsSolid && !tile.Destroyed)
                    return false;
            return true;
        }
    }
}

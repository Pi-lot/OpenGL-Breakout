using OpenGL_Breakout.Enums;
using OpenGL_Breakout.Graphics;
using OpenGL_Breakout.Resources;
using OpenGL_Breakout.Objects;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenGL_Breakout.Structs;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;

namespace OpenGL_Breakout {
    internal class Game {
        public GameState State { get; private set; }
        public bool[] keys { get; set; } = new bool[1024];
        public bool[] keysProcessed { get; set; } = new bool[1024];

        public int Width, Height;

        public List<GameLevel> Levels = new();
        public List<PowerUp> PowerUps = new();
        public int Level;

        public int Lives;

        static Vector2 PLAYER_SIZE = new(100.0f, 20.0f);
        static float PLAYER_VELOCITY = 500.0f;

        static Vector2 INITIAL_BALL_VELOCITY = new(100.0f, -350.0f);
        static float BALL_RADIUS = 12.65f;

        SpriteRenderer Renderer;
        GameObject Player;
        BallObject Ball;
        ParticleGenerator Particles;
        PostProcessor Effects;

        float ShakeTime = 0.0f;

        GCHandle handle = GCHandle.Alloc(DebugCallback);

        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam) {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine("OpenGL Debug: {0} | {1}", msg, type);
            Console.WriteLine(source);
            if (source == DebugSource.DebugSourceApi) {
                
            }
            string? usrParam = Marshal.PtrToStringAnsi(userParam);
            if (usrParam != null)
                Console.WriteLine("UserParam: {0}", usrParam);
            else
                Console.WriteLine("UserParam NULL");

            if (type == DebugType.DebugTypeError) {
                ErrorCode error = GL.GetError();
                if (error != ErrorCode.NoError) {
                    Console.WriteLine("OpenGL Error: {0}", error);
                    switch (error) {
                        case ErrorCode.InvalidOperation:
                            throw new InvalidOperationException(msg);
                        case ErrorCode.OutOfMemory:
                            throw new OutOfMemoryException(msg);
                        case ErrorCode.InvalidValue:
                            throw new ArgumentException(msg);
                        default:
                            throw new Exception(msg);
                    }
                }
            }
        }

        public Game(int width, int height) {
            State = GameState.GAME_MENU;
            Width = width;
            Height = height;
            Level = 0;
            Lives = 3;

            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
            GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
        }

        public void Init() {
            ResourceManager.LoadShader("Shaders/sprite.vert", "Shaders/sprite.frag", null, "sprite");
            ResourceManager.LoadShader("Shaders/particle.vert", "Shaders/particle.frag", null, "particle");
            ResourceManager.LoadShader("Shaders/post_process.vert", "Shaders/post_process.frag", null, "postprocessing");

            Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(0.0f, (float)Width, (float)Height, 0.0f, -1.0f, 1.0f);

            ResourceManager.GetShader("sprite").SetInteger("image", 0, true);
            ResourceManager.GetShader("sprite").SetMatrix4x4("projection", projection);
            ResourceManager.GetShader("particle").SetInteger("image", 0, true);
            ResourceManager.GetShader("particle").SetMatrix4x4("projection", projection);

            Renderer = new(ResourceManager.GetShader("sprite"));
            Effects = new(ResourceManager.GetShader("postprocessing"), Width, Height);

            ResourceManager.LoadTexture("Textures/background.jpg", false, "background");
            ResourceManager.LoadTexture("Textures/awesomeface.png", true, "face");
            ResourceManager.LoadTexture("Textures/block.png", false, "block");
            ResourceManager.LoadTexture("Textures/block_solid.png", false, "block_solid");
            ResourceManager.LoadTexture("Textures/paddle.png", true, "paddle");
            ResourceManager.LoadTexture("Textures/particle.png", true, "particle");
            ResourceManager.LoadTexture("Textures/powerup_speed.png", true, "powerup_speed");
            ResourceManager.LoadTexture("Textures/powerup_sticky.png", true, "powerup_sticky");
            ResourceManager.LoadTexture("Textures/powerup_passthrough.png", true, "powerup_passthrough");
            ResourceManager.LoadTexture("Textures/powerup_increase.png", true, "powerup_increase");
            ResourceManager.LoadTexture("Textures/powerup_confuse.png", true, "powerup_confuse");
            ResourceManager.LoadTexture("Textures/powerup_chaos.png", true, "powerup_chaos");

            GameLevel one = new();
            one.Load("Levels/one.lvl", Width, this.Height / 2);
            GameLevel two = new();
            two.Load("Levels/two.lvl", Width, this.Height / 2);
            GameLevel three = new();
            three.Load("Levels/three.lvl", Width, this.Height / 2);
            GameLevel four = new();
            four.Load("Levels/four.lvl", Width, this.Height / 2);

            Levels.Add(one);
            Levels.Add(two);
            Levels.Add(three);
            Levels.Add(four);
            Level = 0;

            Vector2 playerPos = new(Width / 2.0f - PLAYER_SIZE.X / 2.0f, Height - PLAYER_SIZE.Y);
            Player = new(playerPos, PLAYER_SIZE, ResourceManager.GetTexture("paddle"), Vector3.One);

            Vector2 ballPos = playerPos + new Vector2(PLAYER_SIZE.X / 2.0f - BALL_RADIUS, -BALL_RADIUS * 2.0f);
            Ball = new(ballPos, BALL_RADIUS, INITIAL_BALL_VELOCITY, ResourceManager.GetTexture("face"));

            Particles = new(ResourceManager.GetShader("particle"), ResourceManager.GetTexture("particle"), 1500);
        }

        public void ProcessInput(float dt) {
            if (State == GameState.GAME_MENU) {
                if (keys[(int)Keys.Enter] && !keysProcessed[(int)Keys.Enter]) {
                    State = GameState.GAME_ACTIVE;
                    keysProcessed[(int)Keys.Enter] = true;
                }
                if (keys[(int)Keys.W] && !keysProcessed[(int)Keys.W]) {
                    Level = (Level + 1) % 4;
                    keysProcessed[(int)Keys.W] = true;
                }
                if (keys[(int)Keys.S] && !keysProcessed[(int)Keys.S]) {
                    if (Level > 0)
                        Level--;
                    else
                        Level = 3;
                    keysProcessed[(int)Keys.S] = true;
                }
            }

            if (State == GameState.GAME_ACTIVE) {
                float velocity = PLAYER_VELOCITY * dt;
                if (keys[(int)Keys.A]) {
                    if (Player.Position.X >= 0.0f) {
                        Player.Position.X -= velocity;
                        if (Ball.Stuck)
                            Ball.Position.X -= velocity;
                    }
                }
                if (keys[(int)Keys.D]) {
                    if (Player.Position.X <= Width - Player.Size.X) {
                        Player.Position.X += velocity;
                        if (Ball.Stuck)
                            Ball.Position.X += velocity;
                    }
                }
                if (keys[(int)Keys.Space])
                    Ball.Stuck = false;
            }

            if (State == GameState.GAME_WIN) {
                if (keys[(int)Keys.Enter]) {
                    keysProcessed[(int)Keys.Enter] = true;
                    Effects.Chaos = false;
                    State = GameState.GAME_MENU;
                }
            }
        }

        public void Update(float dt) {
            Ball.Move(dt, Width);
            DoCollisions();

            Particles.Update(dt, Ball, 2, new Vector2(Ball.Radius / 2.0f));

            UpdatePowerUps(dt);

            if (Ball.Position.Y >= Height) {
                Lives--;
                
                Console.WriteLine("Lives Left: {0}", Lives);

                if (Lives == 0) {
                    Console.WriteLine("Out of Lives. Return to Game Menu");

                    ResetLevel();
                    State = GameState.GAME_MENU;
                }

                ResetPlayer();
            }

            if (ShakeTime > 0) {
                ShakeTime -= dt;
                if (ShakeTime <= 0.0f)
                    Effects.Shake = false;
            }

            if (State == GameState.GAME_ACTIVE && Levels[Level].IsCompleted()) {
                ResetLevel();
                ResetPlayer();
                Effects.Chaos = true;
                State = GameState.GAME_WIN;

                Console.WriteLine("You WON!!!!");
                Console.WriteLine("Press ENTER to retry or ESC to quit");
            }
        }

        public void Render(float time) {
            if (State == GameState.GAME_ACTIVE || State == GameState.GAME_MENU || State == GameState.GAME_WIN) {
                Effects.BeginRender();
                Renderer.DrawSprite(ResourceManager.GetTexture("background"),
                    Vector2.Zero, new Vector2(Width, Height), 0.0f, Vector3.One);
                Levels[Level].Draw(Renderer);
                Player.Draw(Renderer);

                foreach (PowerUp powerup in PowerUps)
                    if (!powerup.Destroyed)
                        powerup.Draw(Renderer);

                Particles.Draw();

                Ball.Draw(Renderer);

                Effects.EndRender();
                Effects.Render(time);
            }

            if (State == GameState.GAME_MENU) {
                // Render Menu Text (Not Implemented)
            }

            if (State == GameState.GAME_WIN) {
                // Render Win Text (Not Implemented)
            }
        }

        private float clamp(float value, float min, float max) {
            return Math.Clamp(value, min, max);
        }

        private Direction VectorDirection(Vector2 target) {
            Vector2[] compass = {
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, -1.0f),
                new Vector2(-1.0f, 0.0f)
            };

            float max = 0.0f;

            int best_match = -1;

            for (int i = 0; i < 4; i++) {
                float dot_product = Vector2.Dot(Vector2.Normalize(target), compass[i]);
                if (dot_product > max) {
                    max = dot_product;
                    best_match = i;
                }
            }

            return (Direction)best_match;
        }

        private void ActivatePowerUp(PowerUp powerUp) {
            if (powerUp.Type == "speed") {
                Ball.Velocity *= 1.2f;
            } else if (powerUp.Type == "sticky") {
                Ball.Sticky = true;
                Player.Colour = new Vector3(1.0f, 0.5f, 1.0f);
            } else if (powerUp.Type == "pass-through") {
                Ball.PassThrough = true;
                Ball.Colour = new Vector3(1.0f, 0.5f, 0.5f);
            } else if (powerUp.Type == "pad-size-increase") {
                Player.Size.X += 50.0f;
            } else if (powerUp.Type == "confuse") {
                if (!Effects.Chaos)
                    Effects.Confuse = true;
            } else if (powerUp.Type == "chaos") {
                if (!Effects.Confuse)
                    Effects.Chaos = true;
            }
        }

        private bool CheckCollision(GameObject one, GameObject two) {
            bool collisionX = one.Position.X + one.Size.X >= two.Position.X &&
                two.Position.X + two.Size.X >= one.Position.X;
            bool collisionY = one.Position.Y + one.Size.Y >= two.Position.Y &&
                two.Position.Y + two.Size.Y >= one.Position.Y;
            return collisionX && collisionY;
        }

        private Collision CheckCollision(BallObject one, GameObject two) {
            Vector2 centre = Vector2.Zero;
            centre.X = one.Position.X + one.Radius;
            centre.Y = one.Position.Y + one.Radius;
            Vector2 aabbHalfExtents = new(two.Size.X / 2.0f, two.Size.Y / 2.0f);
            Vector2 aabbCentre = new(two.Position.X + aabbHalfExtents.X, two.Position.Y + aabbHalfExtents.Y);

            Vector2 difference = centre - aabbCentre;
            Vector2 clamped = Vector2.Clamp(difference, -aabbHalfExtents, aabbHalfExtents);
            Vector2 closest = aabbCentre + clamped;
            difference = closest - centre;
            if (difference.Length() < one.Radius)
                return new Collision(true, VectorDirection(difference), difference);
            else
                return new Collision(false, Direction.UP, Vector2.Zero);
        }

        public void DoCollisions() {
            foreach (GameObject box in Levels[Level].Bricks) {
                if (!box.Destroyed) {
                    Collision collision = CheckCollision(Ball, box);

                    if (collision.collided) {
                        if (!box.IsSolid) {
                            box.Destroyed = true;
                            SpawnPowerUps(box);
                        } else {
                            ShakeTime = 0.05f;
                            Effects.Shake = true;
                        }

                        Direction dir = collision.direction;
                        Vector2 diff_vector = collision.Point;
                        if (!(Ball.PassThrough && !box.IsSolid)) {
                            if (dir == Direction.LEFT || dir == Direction.RIGHT) {
                                Ball.Velocity.X = -Ball.Velocity.X;

                                float penetration = Ball.Radius - Math.Abs(diff_vector.X);
                                if (dir == Direction.LEFT)
                                    Ball.Position.X += penetration;
                                else
                                    Ball.Position.Y -= penetration;
                            } else {
                                Ball.Velocity.Y = -Ball.Velocity.Y;

                                float penetration = Ball.Radius - Math.Abs(diff_vector.Y);
                                if (dir == Direction.UP)
                                    Ball.Position.Y -= penetration;
                                else
                                    Ball.Position.Y += penetration;
                            }
                        }
                    }
                }
            }

            Collision result = CheckCollision(Ball, Player);
            if (!Ball.Stuck && result.collided) {
                float centreBoard = Player.Position.X + Player.Size.X / 2.0f;
                float distance = (Ball.Position.X + Ball.Radius) - centreBoard;
                float percentage = distance / (Player.Size.X / 2.0f);
                float strength = 2.0f;
                Vector2 oldVelocity = Ball.Velocity;
                Ball.Velocity.X = INITIAL_BALL_VELOCITY.X * percentage * strength;
                Ball.Velocity = Vector2.Normalize(Ball.Velocity) * oldVelocity.Length();
                Ball.Velocity.Y = -1.0f * Math.Abs(Ball.Velocity.Y);
                Ball.Stuck = Ball.Sticky;
            }

            foreach (PowerUp powerUp in PowerUps) {
                if (!powerUp.Destroyed) {
                    if (powerUp.Position.Y >= Height)
                        powerUp.Destroyed = true;
                    if (CheckCollision(Player, powerUp)) {
                        ActivatePowerUp(powerUp);
                        powerUp.Destroyed = true;
                        powerUp.Activated = true;
                    }
                }
            }
        }

        public void ResetLevel() {
            if (Level == 0)
                Levels[0].Load("Levels/one.lvl", Width, Height / 2);
            else if (Level == 1)
                Levels[1].Load("Levels/two.lvl", Width, Height / 2);
            else if (Level == 2)
                Levels[2].Load("Levels/three.lvl", Width, Height / 2);
            else if (Level == 3)
                Levels[3].Load("Levels/four.lvl", Width, Height / 2);

            Lives = 3;
        }

        public void ResetPlayer() {
            Player.Size = PLAYER_SIZE;
            Player.Position = new Vector2(Width / 2.0f - PLAYER_SIZE.X / 2.0f, Height - PLAYER_SIZE.Y);
            Ball.Reset(Player.Position + new Vector2(PLAYER_SIZE.X / 2.0f - BALL_RADIUS, -(BALL_RADIUS * 2.0f)), INITIAL_BALL_VELOCITY);

            Effects.Chaos = false;
            Effects.Confuse = false;
            Ball.PassThrough = false;
            Ball.Sticky = false;
            Player.Colour = Vector3.One;
            Ball.Colour = Vector3.One;
        }

        private bool ShouldSpawn(int chance) {
            Random r = new Random();
            int random = r.Next(chance);
            return random == 0;
        }

        public void SpawnPowerUps(GameObject block) {
            if (ShouldSpawn(75))
                PowerUps.Add(
                    new PowerUp("speed", new Vector3(0.5f, 0.5f, 1.0f), 0.0f, block.Position, ResourceManager.GetTexture("powerup_speed"))
                );
            if (ShouldSpawn(75))
                PowerUps.Add(
                    new PowerUp("sticky", new Vector3(1.0f, 0.5f, 1.0f), 20.0f, block.Position, ResourceManager.GetTexture("powerup_sticky"))
                );
            if (ShouldSpawn(75))
                PowerUps.Add(
                    new PowerUp("pass-through", new Vector3(0.5f, 1.0f, 0.5f), 10.0f, block.Position, ResourceManager.GetTexture("powerup_passthrough"))
                );
            if (ShouldSpawn(75))
                PowerUps.Add(
                    new PowerUp("pad-size-increase", new Vector3(1.0f, 0.6f, 0.4f), 0.0f, block.Position, ResourceManager.GetTexture("powerup_increase"))
                );
            if (ShouldSpawn(15))
                PowerUps.Add(
                    new PowerUp("confuse", new Vector3(1.0f, 0.3f, 0.3f), 15.0f, block.Position, ResourceManager.GetTexture("powerup_confuse"))
                );
            if (ShouldSpawn(15))
                PowerUps.Add(
                    new PowerUp("chaos", new Vector3(0.9f, 0.25f, 0.25f), 15.0f, block.Position, ResourceManager.GetTexture("powerup_chaos"))
                );
        }

        private bool IsOtherPowerUpActive(List<PowerUp> powerUps, string type) {
            foreach (PowerUp powerUp in powerUps)
                if (powerUp.Activated)
                    if (powerUp.Type == type)
                        return true;

            return false;
        }

        public void UpdatePowerUps(float dt) {
            foreach (PowerUp powerUp in PowerUps) {
                powerUp.Position += powerUp.Velocity * dt;
                if (powerUp.Activated) {
                    powerUp.Duration -= dt;

                    if (powerUp.Duration <= 0.0f) {
                        powerUp.Activated = false;
                        if (powerUp.Type == "sticky") {
                            if (!IsOtherPowerUpActive(PowerUps, "sticky")) {
                                Ball.Sticky = false;
                                Player.Colour = Vector3.One;
                            }
                        } else if (powerUp.Type == "pass-through") {
                            if (!IsOtherPowerUpActive(PowerUps, "pass-through")) {
                                Ball.PassThrough = false;
                                Ball.Colour = Vector3.One;
                            }
                        } else if (powerUp.Type == "confuse") {
                            if (!IsOtherPowerUpActive(PowerUps, "confuse")) {
                                Effects.Confuse = false;
                            }
                        } else if (powerUp.Type == "chaos") {
                            if (!IsOtherPowerUpActive(PowerUps, "chaos")) {
                                Effects.Chaos = false;
                            }
                        }
                    }
                }
            }

            PowerUps.RemoveAll(powerUp => powerUp.Destroyed && !powerUp.Activated);
        }

        public void Close(CancelEventArgs e) {
            ResourceManager.Clear();

            Renderer.Dispose();
            handle.Free();
        }
    }
}
// See https://aka.ms/new-console-template for more information
using OpenGL_Breakout;

Console.WriteLine("Hello, World!");
using (GLWindow window = new (800,600, "Breakout"))
    window.Run();
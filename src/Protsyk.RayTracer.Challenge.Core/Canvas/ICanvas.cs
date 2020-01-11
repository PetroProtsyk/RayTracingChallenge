using System;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public interface ICanvas
    {
        int Width { get; }

        int Height { get; }

        void SetPixel(int x, int y, Color color);

        Color GetPixel(int x, int y);
    }
}
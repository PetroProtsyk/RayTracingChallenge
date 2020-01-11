using System;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public class MemoryCanvas : ICanvas
    {
        private readonly Color[,] m;

        public MemoryCanvas(int width, int height)
        {
            m = new Color[width, height];
        }

        #region ICanvas
        public int Width => m.GetLength(0);

        public int Height => m.GetLength(1);

        public void SetPixel(int x, int y, Color color)
        {
            m[x, y] = color;
        }

        public Color GetPixel(int x, int y)
        {
            return m[x, y];
        }
        #endregion
    }
}
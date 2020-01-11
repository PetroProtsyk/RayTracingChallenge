using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public static class CanvasConverters
    {
        public static readonly ICanvasConverter PPM_P3 = new CanvasToP3Converter();

        public static readonly ICanvasConverter PPM_P6 = new CanvasToP6Converter();
    }
}
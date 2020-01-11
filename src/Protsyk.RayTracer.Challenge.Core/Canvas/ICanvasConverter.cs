using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public interface ICanvasConverter
    {
        void Convert(ICanvas canvas, Stream output);
    }
}
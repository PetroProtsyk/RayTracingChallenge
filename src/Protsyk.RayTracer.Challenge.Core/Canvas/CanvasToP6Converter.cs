using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public class CanvasToP6Converter : ICanvasConverter
    {
        public void Convert(ICanvas canvas, Stream output)
        {
            using (var writerB = new BinaryWriter(output, Encoding.ASCII, true))
            {
                writerB.Write(Encoding.ASCII.GetBytes($"P6 {canvas.Width} {canvas.Height} 255\n"));
                for (int j = 0; j < canvas.Height; ++j)
                {
                    for (int i = 0; i < canvas.Width; ++i)
                    {
                        var color = canvas.GetPixel(i, j);

                        writerB.Write(color.R);
                        writerB.Write(color.G);
                        writerB.Write(color.B);
                    }
                }
                writerB.Write(Encoding.ASCII.GetBytes($"\n"));
            }
        }
    }
}
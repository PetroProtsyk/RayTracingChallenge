using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public class CanvasToP3Converter : ICanvasConverter
    {
        public void Convert(ICanvas canvas, Stream output)
        {
            var line = new StringBuilder();
            using (var writer = new StreamWriter(output, Encoding.ASCII, 4096, true))
            {
                writer.Write($"P3\n{canvas.Width} {canvas.Height}\n255\n");

                for (int j = 0; j < canvas.Height; ++j)
                {
                    for (int i = 0; i < canvas.Width; ++i)
                    {
                        var color = canvas.GetPixel(i, j);
                        AppendFlush(line, 70, writer, color.R);
                        AppendFlush(line, 70, writer, color.G);
                        AppendFlush(line, 70, writer, color.B);
                    }
                    if (line.Length > 0)
                    {
                        writer.Write(line);
                        writer.Write("\n");
                        line.Clear();
                    }
                }

                if (line.Length > 0)
                {
                    writer.Write(line);
                    writer.Write("\n");
                    line.Clear();
                }
            }
        }

        private void AppendFlush(StringBuilder line, int maxLength, TextWriter writer, byte value)
        {
            var newLen = 0;
            if (line.Length > 0)
            {
                newLen += 1; // Space
            }
            if (value < 10)
            {
                newLen += 1;
            }
            else if (value < 100)
            {
                newLen += 2;
            }
            else
            {
                newLen += 3;
            }

            if (line.Length + newLen > maxLength)
            {
                writer.Write(line);
                writer.Write("\n");
                line.Clear();
            }

            if (line.Length > 0)
            {
                line.Append(' ');
            }
            line.Append(value);
        }
    }
}
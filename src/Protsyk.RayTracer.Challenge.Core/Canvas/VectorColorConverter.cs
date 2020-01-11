using System;
using System.Numerics;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public class VectorColorConverter : IColorConverter<Vector3>
    {
        public Color From(Vector3 color)
        {
            return new Color((byte)color.X,
                             (byte)color.Y,
                             (byte)color.Z);
        }
    }
}
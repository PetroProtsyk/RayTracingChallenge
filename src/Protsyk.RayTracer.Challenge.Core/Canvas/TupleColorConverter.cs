using System;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public class TupleColorConverter : IColorConverter<Tuple4>
    {
        public Color From(Tuple4 color)
        {
            return new Color((byte)Math.Min(255, (int)Math.Max(0, 256.0*color.X)),
                             (byte)Math.Min(255, (int)Math.Max(0, 256.0*color.Y)),
                             (byte)Math.Min(255, (int)Math.Max(0, 256.0*color.Z)));
        }
    }
}
using System;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public static class ColorConverters
    {
        // Converts from [0..1] to [0..255]
        public static readonly IColorConverter<Tuple4> Tuple1 = new TupleColorConverter();

        // Converts from [0..255] to [0..255]
        public static readonly IColorConverter<Tuple4> Tuple255 = new VectorColorConverter();
    }
}
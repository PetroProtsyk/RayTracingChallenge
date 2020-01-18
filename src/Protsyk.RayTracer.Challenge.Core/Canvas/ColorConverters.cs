using System;
using System.Numerics;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public static class ColorConverters
    {
        public static readonly IColorConverter<Tuple4> Tuple4 = new TupleColorConverter();

        public static readonly IColorConverter<Vector3> Vector3 = new VectorColorConverter();
    }
}
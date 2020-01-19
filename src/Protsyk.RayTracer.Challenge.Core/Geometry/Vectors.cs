using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public static class Vectors
    {
        public static Tuple4 V(double x, double y, double z)
        {
            return new Tuple4(x, y, z, TupleFlavour.Vector);
        }

        public static Tuple4 P(double x, double y, double z)
        {
            return new Tuple4(x, y, z, TupleFlavour.Point);
        }
    }
}

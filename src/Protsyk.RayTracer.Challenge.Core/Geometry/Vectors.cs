using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public static class Vectors
    {
        public static Vector3 V(double x, double y, double z)
        {
            return V((float)x, (float)y, (float)z);
        }

        public static Vector3 V(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }
    }
}

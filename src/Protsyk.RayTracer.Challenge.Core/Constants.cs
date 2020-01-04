using System;

namespace Protsyk.RayTracer.Challenge.Core
{
    public static class Constants
    {
        public static double Epsilon => 0.00001;

        public static bool EpsilonCompare(double a, double b)
        {
            return Math.Abs(a - b) < Epsilon;
        }
    }
}
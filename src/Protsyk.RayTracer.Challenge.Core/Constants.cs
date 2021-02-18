using System;

namespace Protsyk.RayTracer.Challenge.Core
{
    public static class Constants
    {
        public static double Epsilon => 0.00001;

        /// <summary>
        /// Epsilon / 2
        /// </summary>
        public static double HalfEpsilon => 0.000005;

        public static bool EpsilonCompare(double a, double b)
        {
            return Math.Abs(a - b) < Epsilon;
        }
    }
}
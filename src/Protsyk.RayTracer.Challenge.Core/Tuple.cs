using System;

namespace Protsyk.RayTracer.Challenge.Core
{
    public class Tuple4
    {

        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly double W;

        public Tuple4(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public static Tuple4 Add(Tuple4 a, Tuple4 b)
        {
            return new Tuple4(
                    a.X + b.X,
                    a.Y + b.Y,
                    a.Z + b.Z,
                    a.W + b.W
            );
        }

        public static Tuple4 Subtract(Tuple4 a, Tuple4 b)
        {
            return new Tuple4(
                    a.X - b.X,
                    a.Y - b.Y,
                    a.Z - b.Z,
                    a.W - b.W
            );
        }

        public static Tuple4 Negate(Tuple4 a)
        {
            return new Tuple4(
                    -a.X,
                    -a.Y,
                    -a.Z,
                    -a.W
            );
        }

        public static Tuple4 Scale(Tuple4 a, double b)
        {
            return new Tuple4(
                    a.X * b,
                    a.Y * b,
                    a.Z * b,
                    a.W * b
            );
        }

        // Hadamard product
        public static Tuple4 Scale(Tuple4 a, Tuple4 b)
        {
            return new Tuple4(
                    a.X * b.X,
                    a.Y * b.Y,
                    a.Z * b.Z,
                    a.W * b.W
            );
        }

        public static Tuple4 Normalize(Tuple4 a)
        {
            return Scale(a, 1 / a.Length());
        }

        public static double DotProduct(Tuple4 a, Tuple4 b)
        {
            return  a.X * b.X +
                    a.Y * b.Y +
                    a.Z * b.Z +
                    a.W * b.W;
        }

        public static Tuple4 CrossProduct(Tuple4 a, Tuple4 b)
        {
            return new Tuple4(
                    a.Y * b.Z - a.Z * b.Y,
                    a.Z * b.X - a.X * b.Z,
                    a.X * b.Y - a.Y * b.X,
                    0
            );
        }

        public static Tuple4 Reflect(Tuple4 direction, Tuple4 normal) {
            return Tuple4.Subtract(direction, Tuple4.Scale(normal, 2 * Tuple4.DotProduct(direction, normal)));
        }

        public double Length() {
            return Math.Sqrt(X*X + Y*Y + Z*Z + W*W);
        }
    }
}

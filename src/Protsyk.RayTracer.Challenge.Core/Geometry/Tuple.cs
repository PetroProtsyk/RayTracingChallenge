using System;
using System.Diagnostics;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Tuple4 : IEquatable<Tuple4>
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly double W;

        public static readonly Tuple4 ZeroVector = new Tuple4(0.0, 0.0, 0.0, TupleFlavour.Vector);

        public static readonly Tuple4 ZeroPoint = new Tuple4(0.0, 0.0, 0.0, TupleFlavour.Point);

        [DebuggerStepThrough]
        public Tuple4(double x, double y, double z, TupleFlavour type)
            : this(x, y, z, type == TupleFlavour.Point ? 1.0 : 0.0)
        {
        }

        [DebuggerStepThrough]
        public Tuple4(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        [DebuggerStepThrough]
        public static Tuple4 Point(double x, double y, double z) => new Tuple4(x, y, z, TupleFlavour.Point);

        [DebuggerStepThrough]
        public static Tuple4 Vector(double x, double y, double z) => new Tuple4(x, y, z, TupleFlavour.Vector);

        [DebuggerStepThrough]
        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}, {W}]";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public override bool Equals(object obj)
        {
            var m = obj as Tuple4;
            return (m != null) && Equals(m);
        }

        public bool Equals(Tuple4 other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return Constants.EpsilonCompare(X, other.X) &&
                   Constants.EpsilonCompare(Y, other.Y) &&
                   Constants.EpsilonCompare(Z, other.Z) &&
                   Constants.EpsilonCompare(W, other.W);
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
            return Scale(a, 1.0 / a.Length());
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
                    TupleFlavour.Vector
            );
        }

        public static Tuple4 Reflect(Tuple4 direction, Tuple4 normal) {
            return Tuple4.Subtract(direction, Tuple4.Scale(normal, 2.0 * Tuple4.DotProduct(direction, normal)));
        }

        public double Length() {
            if (!IsVector()) {
                throw new NotSupportedException();
            }
            return Math.Sqrt(X*X + Y*Y + Z*Z + W*W);
        }

        public bool IsVector() {
            return Constants.EpsilonZero(W);
        }

        public bool IsPoint() {
            return !IsVector();
        }

        public static class Geometry3D
        {
            public static Tuple4 MovePoint(Tuple4 origin, Tuple4 dir, double t)
            {
                if (!dir.IsVector())
                {
                    throw new ArgumentException("Not a vector");
                }
                return new Tuple4(
                        origin.X + t * dir.X,
                        origin.Y + t * dir.Y,
                        origin.Z + t * dir.Z,
                        TupleFlavour.Point
                );
            }

        }

    }

    public enum TupleFlavour
    {
        Vector,
        Point
    }
}

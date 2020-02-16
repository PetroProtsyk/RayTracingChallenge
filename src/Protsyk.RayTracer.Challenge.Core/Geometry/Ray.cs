using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Ray
    {
        public readonly Tuple4 origin;
        public readonly Tuple4 dir;

        public Ray(Tuple4 origin, Tuple4 dir)
        {
            if (!origin.IsPoint())
            {
                throw new ArgumentException("Not a point", nameof(origin));
            }
            if (!dir.IsVector())
            {
                throw new ArgumentException("Not a vector", nameof(dir));
            }
            this.origin = origin;
            this.dir = dir;
        }

        public Tuple4 PositionAt(double t)
        {
            return Tuple4.Add(origin, Tuple4.Scale(dir, new Tuple4(t, t, t, TupleFlavour.Point)));
        }

        public Ray Transform(IMatrix m)
        {
            var newOrigin = MatrixOperations.Geometry3D.ToTuple(
                MatrixOperations.Multiply(m, MatrixOperations.Geometry3D.FromTuple(origin)));

            var newDir = MatrixOperations.Geometry3D.ToTuple(
                MatrixOperations.Multiply(m, MatrixOperations.Geometry3D.FromTuple(dir)));

            return new Ray(newOrigin, newDir);
        }
    }
}
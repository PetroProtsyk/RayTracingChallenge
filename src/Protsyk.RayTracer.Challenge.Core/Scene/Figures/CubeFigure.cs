using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    /// <summary>
    /// AABB Cube
    /// </summary>
    public class CubeFigure : BaseFigure
    {
        public CubeFigure(IMatrix transformation, IMaterial material)
        {
            this.Material = material;
            this.Transformation = transformation;
        }

        private static readonly Tuple4 OXPlus = Tuple4.Vector(1, 0, 0);
        private static readonly Tuple4 OYPlus = Tuple4.Vector(0, 1, 0);
        private static readonly Tuple4 OZPlus = Tuple4.Vector(0, 0, 1);

        private static readonly Tuple4 OXMinus = Tuple4.Vector(-1, 0, 0);
        private static readonly Tuple4 OYMinus = Tuple4.Vector(0, -1, 0);
        private static readonly Tuple4 OZMinus = Tuple4.Vector(0, 0, -1);

        protected override Tuple4 GetBaseNormal(Tuple4 pointOnSurface)
        {
            var absX = Math.Abs(pointOnSurface.X);
            var absY = Math.Abs(pointOnSurface.Y);
            var absZ = Math.Abs(pointOnSurface.Z);

            var maxc = Math.Max(absX, Math.Max(absY, absZ));

            if (maxc == absX)
            {
                return pointOnSurface.X > 0.0 ? OXPlus : OXMinus;
            }

            if (maxc == absY)
            {
                return pointOnSurface.Y > 0.0 ? OYPlus : OYMinus;
            }

            return pointOnSurface.Z > 0.0 ? OZPlus : OZMinus;
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            var (xtmin, xtmax) = CheckAxis(ray.origin.X, ray.dir.X);
            var (ytmin, ytmax) = CheckAxis(ray.origin.Y, ray.dir.Y);
            var (ztmin, ztmax) = CheckAxis(ray.origin.Z, ray.dir.Z);

            var tmin = Math.Max(xtmin, Math.Max(ytmin, ztmin));
            var tmax = Math.Min(xtmax, Math.Min(ytmax, ztmax));

            if (tmin > tmax)
            {
                return null;
            }

            return new Intersection[] { new Intersection(tmin, this), new Intersection(tmax, this) };
        }

        private (double, double) CheckAxis(double origin, double direction)
        {
            var tmin_numerator = -1.0 - origin;
            var tmax_numerator = 1.0 - origin;

            double tmin;
            double tmax;

            if (Constants.EpsilonZero(Math.Abs(direction)))
            {
                tmin = tmin_numerator >= 0.0 ? double.PositiveInfinity : double.NegativeInfinity;
                tmax = tmax_numerator >= 0.0 ? double.PositiveInfinity : double.NegativeInfinity;
            }
            else
            {
                tmin = tmin_numerator / direction;
                tmax = tmax_numerator / direction;
            }

            if (tmin > tmax)
            {
                return (tmax, tmin);
            }
            else
            {
                return (tmin, tmax);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is CubeFigure figure &&
                   EqualityComparer<IMaterial>.Default.Equals(Material, figure.Material) &&
                   EqualityComparer<IMatrix>.Default.Equals(Transformation, figure.Transformation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Material.GetHashCode(), Transformation.GetHashCode());
        }
    }

}

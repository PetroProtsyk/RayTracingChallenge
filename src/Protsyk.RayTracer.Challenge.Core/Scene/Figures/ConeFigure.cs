using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    /// <summary>
    /// Cone (with optional Caps)
    /// </summary>
    public class ConeFigure : BaseFigure
    {
        public double Minimum { get; set;  }
        public double Maximum { get; set; }
        public Boolean IsClosed { get; set; }

        public ConeFigure(IMatrix transformation, IMaterial material)
            : this(transformation, material, double.NegativeInfinity, double.PositiveInfinity, false)
        {
        }

        public ConeFigure(IMatrix transformation, IMaterial material, double min, double max, bool isClosed)
        {
            this.Material = material;
            this.Transformation = transformation;
            this.Minimum = min;
            this.Maximum = max;
            this.IsClosed = isClosed;
        }

        protected override Tuple4 GetBaseNormal(Tuple4 pointOnSurface)
        {
            var d = pointOnSurface.X * pointOnSurface.X + pointOnSurface.Z * pointOnSurface.Z;

            if ((d < 1) && (pointOnSurface.Y + Constants.Epsilon >= Maximum))
            {
                return Tuple4.Vector(0, 1, 0);
            }
            else if ((d < 1) && (pointOnSurface.Y - Constants.Epsilon <= Minimum))
            {
                return Tuple4.Vector(0, -1, 0);
            }

            var y = Math.Sqrt(pointOnSurface.X * pointOnSurface.X + pointOnSurface.Z * pointOnSurface.Z);
            if (pointOnSurface.Y > 0)
            {
                y = -y;
            }

            return Tuple4.Vector(pointOnSurface.X, y, pointOnSurface.Z);
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            return SharedUtils.JoinArrays(GetConeIntersections(ray), GetCapsIntersections(ray));
        }

        private Intersection[] GetConeIntersections(Ray ray)
        { 
            var a = ray.dir.X * ray.dir.X - ray.dir.Y * ray.dir.Y + ray.dir.Z * ray.dir.Z;
            var b = 2 * (ray.origin.X * ray.dir.X - ray.origin.Y * ray.dir.Y + ray.origin.Z * ray.dir.Z);
            var c = ray.origin.X * ray.origin.X - ray.origin.Y * ray.origin.Y + ray.origin.Z * ray.origin.Z;

            if (Constants.EpsilonZero(a) && Constants.EpsilonZero(b))
            {
                return null;
            }

            if (Constants.EpsilonZero(a))
            {
                var t = -c / (2 * b);
                var yt = ray.origin.Y + t * ray.dir.Y;
                if (Minimum < yt && yt < Maximum)
                {
                    return new Intersection[] { new Intersection(t, this) };
                }
            }

            var d = b * b - 4 * a * c;
            if (d < 0.0)
            {
                return null;
            }

            var a2 = a * 2;
            var dSqrt = Math.Sqrt(d);
            var t0 = (-b - dSqrt) / a2;
            var t1 = (-b + dSqrt) / a2;

            if (t0 > t1)
            {
                var t = t0;
                t0 = t1;
                t1 = t;
            }

            var isT0 = false;
            var y0 = ray.origin.Y + t0 * ray.dir.Y;
            if (Minimum < y0 && y0 < Maximum)
            {
                isT0 = true;
            }

            var isT1 = false;
            var y1 = ray.origin.Y + t1 * ray.dir.Y;
            if (Minimum < y1 && y1 < Maximum)
            {
                isT1 = true;
            }

            if (isT0 && isT1)
            {
                return new Intersection[] { new Intersection(t0, this), new Intersection(t1, this) };
            }
            else if (isT0)
            {
                return new Intersection[] { new Intersection(t0, this) };
            }
            else if (isT1)
            {
                return new Intersection[] { new Intersection(t1, this) };
            }
            return null;
        }

        private Intersection[] GetCapsIntersections(Ray ray)
        {
            if (!IsClosed || Constants.EpsilonZero(ray.dir.Y))
            {
                return null;
            }

            var t0 = (Minimum - ray.origin.Y) / ray.dir.Y;
            var t1 = (Maximum - ray.origin.Y) / ray.dir.Y;

            var isT0 = IsOnCap(ray, t0);
            var isT1 = IsOnCap(ray, t1);

            if (isT0 && isT1)
            {
                return new Intersection[] { new Intersection(t0, this), new Intersection(t1, this) };
            }
            else if (isT0)
            {
                return new Intersection[] { new Intersection(t0, this) };
            }
            else if (isT1)
            {
                return new Intersection[] { new Intersection(t1, this) };
            }

            return null;
        }

        private bool IsOnCap(Ray r, double t)
        {
            var x = r.origin.X + t * r.dir.X;
            var y = r.origin.Y + t * r.dir.Y;
            var z = r.origin.Z + t * r.dir.Z;
            return (x * x + z * z) <= y * y;
        }

        public override bool Equals(object obj)
        {
            return obj is ConeFigure figure &&
                   EqualityComparer<double>.Default.Equals(Minimum, figure.Minimum) &&
                   EqualityComparer<double>.Default.Equals(Maximum, figure.Maximum) &&
                   EqualityComparer<bool>.Default.Equals(IsClosed, figure.IsClosed) &&
                   EqualityComparer<IMaterial>.Default.Equals(Material, figure.Material) &&
                   EqualityComparer<IMatrix>.Default.Equals(Transformation, figure.Transformation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Material.GetHashCode(), Transformation.GetHashCode());
        }
    }

}
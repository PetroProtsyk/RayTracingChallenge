using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    /// <summary>
    /// Cylinder (with optional Caps)
    /// </summary>
    public class CylinderFigure : BaseFigure
    {
        public double Minimum { get; set;  }
        public double Maximum { get; set; }
        public Boolean IsClosed { get; set; }

        public CylinderFigure(IMatrix transformation, IMaterial material)
            : this(transformation, material, double.NegativeInfinity, double.PositiveInfinity, false)
        {
        }

        public CylinderFigure(IMatrix transformation, IMaterial material, double min, double max, bool isClosed)
        {
            this.Material = material;
            this.Transformation = transformation;
            this.Minimum = min;
            this.Maximum = max;
            this.IsClosed = isClosed;
        }

        protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface)
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

            return Tuple4.Vector(pointOnSurface.X, 0.0, pointOnSurface.Z);
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            var a = ray.dir.X * ray.dir.X + ray.dir.Z * ray.dir.Z;

            if (Constants.EpsilonZero(a))
            {
                return GetCapsIntersections(ray);
            }

            var b = 2 * (ray.origin.X * ray.dir.X + ray.origin.Z * ray.dir.Z);
            var c = ray.origin.X * ray.origin.X + ray.origin.Z * ray.origin.Z - 1;

            var d = b * b - 4 * a * c;
            if (d < 0.0)
            {
                return GetCapsIntersections(ray);
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

            var capsT = GetCapsIntersections(ray);

            var len = capsT == null ? 0 : capsT.Length;
            if (isT0 && isT1)
            {
                len += 2;
            }
            else if (isT0)
            {
                len += 1;
            }
            else if (isT1)
            {
                len += 1;
            }

            if (len == 0)
            {
                return null;
            }

            var r = new Intersection[len];
            var w = 0;
            if (capsT != null)
            {
                if (capsT.Length == 1)
                {
                    r[0] = capsT[0];
                    w = 1;
                }
                else if (capsT.Length == 2)
                {
                    r[0] = capsT[0];
                    r[1] = capsT[1];
                    w = 2;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            if (isT0 && isT1)
            {
                r[w] = new Intersection(t0, this);
                r[w + 1] = new Intersection(t1, this);
            }
            else if (isT0)
            {
                r[w] = new Intersection(t0, this);
            }
            else if (isT1)
            {
                r[w] = new Intersection(t1, this);
            }

            return r;
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
            var z = r.origin.Z + t * r.dir.Z;
            return (x * x + z * z) <= 1.0;
        }

        public override bool Equals(object obj)
        {
            return obj is CylinderFigure figure &&
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
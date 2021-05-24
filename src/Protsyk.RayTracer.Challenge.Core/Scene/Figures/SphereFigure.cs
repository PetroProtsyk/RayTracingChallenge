using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{

    public class SphereFigure : BaseFigure
    {
        private readonly Sphere sphere;

        public SphereFigure(Tuple4 center, double radius, IMaterial material)
        {
            this.sphere = new Sphere(center, radius);
            this.Material = material;
        }

        public SphereFigure(IMatrix transformation, IMaterial material)
        {
            this.sphere = new Sphere();
            this.Material = material;
            this.Transformation = transformation;
        }

        public SphereFigure(Sphere sphere, IMaterial material)
        {
            this.sphere = sphere;
            this.Material = material;
        }

        protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface, double u, double v)
        {
            return sphere.GetNormal(pointOnSurface);
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            var xs = sphere.GetIntersections(ray);
            if (xs == null)
            {
                return null;
            }
            var r = new Intersection[xs.Length];
            for (int i = 0; i < xs.Length; ++i)
            {
                r[i] = new Intersection(xs[i], this);
            }
            return r;
        }

        public override bool Equals(object obj)
        {
            return obj is SphereFigure figure &&
                   EqualityComparer<Sphere>.Default.Equals(sphere, figure.sphere) &&
                   EqualityComparer<IMaterial>.Default.Equals(Material, figure.Material) &&
                   EqualityComparer<IMatrix>.Default.Equals(Transformation, figure.Transformation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(sphere, Material.GetHashCode(), Transformation.GetHashCode());
        }
    }

}

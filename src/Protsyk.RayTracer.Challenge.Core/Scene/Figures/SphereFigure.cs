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

        protected override Tuple4 GetBaseNormal(Tuple4 pointOnSurface)
        {
            return sphere.GetNormal(pointOnSurface);
        }

        protected override double[] GetBaseIntersections(Ray ray)
        {
            return sphere.GetIntersections(ray);
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

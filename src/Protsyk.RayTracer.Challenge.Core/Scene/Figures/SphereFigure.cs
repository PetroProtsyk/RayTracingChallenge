using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{

    public class SphereFigure : BaseFigure
    {
        private readonly Sphere sphere;

        private IMaterial material;

        public SphereFigure(Tuple4 center, double radius, IMaterial material)
        {
            this.sphere = new Sphere(center, radius);
            this.material = material;
        }

        public SphereFigure(IMatrix transformation, IMaterial material)
        {
            this.sphere = new Sphere(transformation);
            this.material = material;
        }

        public SphereFigure(Sphere sphere, IMaterial material)
        {
            this.sphere = sphere;
            this.material = material;
        }

        public override IMaterial GetMaterial()
        {
            return material;
        }

        public override void SetMaterial(IMaterial material)
        {
            this.material = material;
        }

        public override IMatrix GetTransformation()
        {
            return sphere.Transformation;
        }

        public override Tuple4 GetNormal(Tuple4 pointOnSurface)
        {
            return sphere.GetNormal(pointOnSurface);
        }

        public override double[] GetIntersections(Ray ray)
        {
            return sphere.GetIntersections(ray);
        }

        public override bool Equals(object obj)
        {
            return obj is SphereFigure figure &&
                   EqualityComparer<Sphere>.Default.Equals(sphere, figure.sphere) &&
                   EqualityComparer<IMaterial>.Default.Equals(material, figure.material);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(sphere, material);
        }
    }

}

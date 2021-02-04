using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{

    public class SphereFigure : IFigure
    {
        private readonly Sphere sphere;

        private readonly IMaterial material;

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

        public IMaterial GetMaterial()
        {
            return material;
        }

        public IMatrix GetTransformation()
        {
            return sphere.Transformation;
        }

        public Tuple4 GetNormal(Tuple4 pointOnSurface)
        {
            return sphere.GetNormal(pointOnSurface);
        }

        public HitResult Hit(Tuple4 origin, Tuple4 dir)
        {
            return HitResult.ClosestPositiveHit(AllHits(origin, dir));
        }

        public HitResult[] AllHits(Tuple4 origin, Tuple4 dir)
        {
            var intersections = sphere.GetIntersections(new Ray(origin, dir));
            if (intersections == null)
            {
                return new HitResult[] { HitResult.NoHit };
            }

            var result = new HitResult[intersections.Length];
            for(int i=0; i<intersections.Length; ++i)
            {
                var distance = intersections[i];
                var pointOnSurface = Tuple4.Geometry3D.MovePoint(origin, dir, distance); // orig + dir*dist
                var surfaceNormal = sphere.GetNormal(pointOnSurface);

                result[i] = new HitResult(true, this, distance, pointOnSurface, surfaceNormal, Tuple4.Negate(dir));
            }

            return result;
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

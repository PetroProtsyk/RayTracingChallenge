using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{

    public class SphereFigure : IFigure
    {
        private readonly Geometry.Sphere sphere;

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

        public Tuple4 ColorAt(HitResult hit)
        {
            if (!hit.IsHit)
            {
                throw new InvalidOperationException();
            }
            return material.GetColor();
        }

        public HitResult Hit(Tuple4 origin, Tuple4 dir)
        {
            var intersections = sphere.GetIntersections(new Ray(origin, dir));
            if (intersections == null)
            {
                return HitResult.NoHit;
            }

            var distance = (intersections[0] < 0) ? intersections[1] : intersections[0];
            if (distance < 0)
            {
                return HitResult.NoHit;
            }

            var pointOnSurface = Tuple4.Add(origin, Tuple4.Scale(dir, distance)); // orig + dir*dist
            var surfaceNormal = sphere.GetNormal(pointOnSurface);

            return new HitResult(true, this, distance, pointOnSurface, surfaceNormal);
        }

        public HitResult[] AllHits(Tuple4 origin, Tuple4 dir)
        {
            var intersections = sphere.GetIntersections(new Ray(origin, dir));
            if (intersections == null)
            {
                return new HitResult[] { HitResult.NoHit };
            }

            var distance1 = intersections[0];
            var pointOnSurface1 = Tuple4.Add(origin, Tuple4.Scale(dir, distance1)); // orig + dir*dist
            var surfaceNormal1 = sphere.GetNormal(pointOnSurface1);

            var distance2 = intersections[1];
            var pointOnSurface2 = Tuple4.Add(origin, Tuple4.Scale(dir, distance2)); // orig + dir*dist
            var surfaceNormal2 = sphere.GetNormal(pointOnSurface2);

            return new HitResult[]
            {
                new HitResult(true, this, distance1, pointOnSurface1, surfaceNormal1),
                new HitResult(true, this, distance2, pointOnSurface2, surfaceNormal2)
            };
        }
    }

}

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

        public IMaterial GetMaterial()
        {
            return material;
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
            var distance = sphere.Intersects(origin, dir);
            if (distance < 0)
            {
                return HitResult.NoHit;
            }

            var pointOnSurface = Tuple4.Add(origin, Tuple4.Scale(dir, distance)); // orig + dir*dist
            var surfaceNormal = Tuple4.Normalize(Tuple4.Subtract(pointOnSurface, sphere.Center)); // hit - Center

            return new HitResult(true, this, distance, pointOnSurface, surfaceNormal);
        }
    }

}

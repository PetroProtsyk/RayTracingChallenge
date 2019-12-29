using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{

    public class SphereFigure : IFigure
    {
        private readonly Geometry.Sphere sphere;

        private readonly IMaterial material;

        public SphereFigure(Vector3 center, float radius, IMaterial material)
        {
            this.sphere = new Sphere(center, radius);
            this.material = material;
        }

        public IMaterial GetMaterial()
        {
            return material;
        }

        public Vector3 ColorAt(HitResult hit)
        {
            if (!hit.IsHit)
            {
                throw new InvalidOperationException();
            }
            return material.GetColor();
        }

        public HitResult Hit(Vector3 origin, Vector3 dir)
        {
            var distance = sphere.Intersects(origin, dir);
            if (distance < 0)
            {
                return HitResult.NoHit;
            }

            var pointOnSurface = Vector3.Add(origin, Vector3.Multiply(distance, dir)); // orig + dir*dist
            var surfaceNormal = Vector3.Normalize(Vector3.Subtract(pointOnSurface, sphere.Center)); // hit - Center

            return new HitResult(true, this, distance, pointOnSurface, surfaceNormal);
        }
    }

}

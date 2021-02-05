using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public abstract class BaseFigure : IFigure
    {
        public abstract Tuple4 GetNormal(Tuple4 pointOnSurface);

        public abstract IMaterial GetMaterial();

        public abstract IMatrix GetTransformation();

        public abstract double[] GetIntersections(Ray ray);

        public virtual HitResult Hit(Tuple4 origin, Tuple4 dir)
        {
            return ClosestPositiveHit(AllHits(origin, dir));
        }

        public virtual HitResult[] AllHits(Tuple4 origin, Tuple4 dir)
        {
            var intersections = GetIntersections(new Ray(origin, dir));
            if (intersections == null)
            {
                return new HitResult[] { HitResult.NoHit };
            }

            var result = new HitResult[intersections.Length];
            for(int i=0; i<intersections.Length; ++i)
            {
                var distance = intersections[i];
                var pointOnSurface = Tuple4.Geometry3D.MovePoint(origin, dir, distance); // orig + dir*dist
                var surfaceNormal = GetNormal(pointOnSurface);
                var eyeVector = Tuple4.Negate(dir);
                var isInside = false;
                if (Tuple4.DotProduct(surfaceNormal, eyeVector) < 0)
                {
                    isInside = true;
                    surfaceNormal = Tuple4.Negate(surfaceNormal);
                }

                result[i] = new HitResult(true, this, distance, pointOnSurface, surfaceNormal, eyeVector, isInside);
            }

            return result;
        }

        public static HitResult ClosestPositiveHit(HitResult[] hits)
        {
            var result = HitResult.NoHit;
            foreach (var hit in hits)
            {
                if (hit.Distance > 0.0)
                {
                    if (result.Equals(HitResult.NoHit))
                    {
                        result = hit;
                    }
                    else if (result.Distance > hit.Distance)
                    {
                        result = hit;
                    }
                }
            }
            return result;
        }
    }

}

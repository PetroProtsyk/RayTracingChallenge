using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{

    public class SDFFigure : IFigure
    {
        private readonly Geometry.SignedDistanceField sdf;

        private readonly IMaterial material;

        public SDFFigure(SignedDistanceField sdf, IMaterial material)
        {
            this.sdf = sdf;
            this.material = material;
        }

        public IMaterial GetMaterial()
        {
            return material;
        }

        public IMatrix GetTransformation()
        {
            return sdf.Transformation;
        }

        public Tuple4 GetNormal(Tuple4 pointOnSurface)
        {
            return sdf.GetNormal(pointOnSurface);
        }

        public HitResult Hit(Tuple4 origin, Tuple4 dir)
        {
            return HitResult.ClosestPositiveHit(AllHits(origin, dir));
        }

        public HitResult[] AllHits(Tuple4 origin, Tuple4 dir)
        {
            var intersections = sdf.GetIntersections(new Ray(origin, dir));
            if (intersections == null)
            {
                return new HitResult[] { HitResult.NoHit };
            }

            var result = new HitResult[intersections.Length];
            for(int i=0; i<intersections.Length; ++i)
            {
                var distance = intersections[i];
                var pointOnSurface = Tuple4.Geometry3D.MovePoint(origin, dir, distance); // orig + dir*dist
                var surfaceNormal = sdf.GetNormal(pointOnSurface);

                result[i] = new HitResult(true, this, distance, pointOnSurface, surfaceNormal);
            }

            return result;
        }
    }

}

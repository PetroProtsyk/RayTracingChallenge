using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public class SDFFigure : BaseFigure
    {
        private readonly Geometry.SignedDistanceField sdf;

        public SDFFigure(SignedDistanceField sdf, IMaterial material)
        {
            this.sdf = sdf;
            this.Material = material;
        }

        protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface, double u, double v)
        {
            return sdf.GetNormal(pointOnSurface);
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            var xs = sdf.GetIntersections(ray);
            if (xs == null)
            {
                return null;
            }
            var r = new Intersection[xs.Length];
            for (int i=0; i<xs.Length; ++i)
            {
                r[i] = new Intersection(xs[i], this);
            }
            return r;
        }
    }

}

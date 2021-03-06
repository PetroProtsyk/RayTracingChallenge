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

        protected override Tuple4 GetBaseNormal(Tuple4 pointOnSurface)
        {
            return sdf.GetNormal(pointOnSurface);
        }

        protected override double[] GetBaseIntersections(Ray ray)
        {
            return sdf.GetIntersections(ray);
        }
    }

}

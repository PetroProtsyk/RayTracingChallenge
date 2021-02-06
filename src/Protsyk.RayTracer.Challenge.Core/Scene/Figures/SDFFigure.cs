using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public class SDFFigure : BaseFigure
    {
        private readonly Geometry.SignedDistanceField sdf;

        private IMaterial material;

        public SDFFigure(SignedDistanceField sdf, IMaterial material)
        {
            this.sdf = sdf;
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
            return sdf.Transformation;
        }

        public override Tuple4 GetNormal(Tuple4 pointOnSurface)
        {
            return sdf.GetNormal(pointOnSurface);
        }

        public override double[] GetIntersections(Ray ray)
        {
            return sdf.GetIntersections(ray);
        }
    }

}

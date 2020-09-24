using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public class AmbientLight : ILight
    {
        private readonly double intensity;

        public AmbientLight(double intensity)
        {
            this.intensity = intensity / 100.0;
        }

        public Tuple4 GetLightDirection(Tuple4 from)
        {
            return Tuple4.ZeroVector;
        }

        public double GetLightDistance(Tuple4 from)
        {
            return 0.0;
        }

        public double GetIntensity(Tuple4 dir, double shine, Tuple4 pointOnSurface, Tuple4 surfaceNormal)
        {
            return intensity;
        }

    }

}

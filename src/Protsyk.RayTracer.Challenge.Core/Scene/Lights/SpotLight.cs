using System;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public class SpotLight : ILight
    {
        private readonly Tuple4 location;

        private readonly double intensity;

        public SpotLight(Tuple4 location, double intensity)
        {
            this.location = location;
            this.intensity = intensity / 100.0;
        }

        public Tuple4 GetLightDirection(Tuple4 from)
        {
            return Tuple4.Normalize(Tuple4.Subtract(location, from));
        }

        public double GetLightDistance(Tuple4 from)
        {
            return Tuple4.Subtract(location, from).Length();
        }

        public double GetIntensity(Tuple4 dir, int shine, Tuple4 pointOnSurface, Tuple4 surfaceNormal)
        {
            var lightDirection = Tuple4.Normalize(Tuple4.Subtract(location, pointOnSurface));
            var result = 0.0;

            if (shine != MaterialConstants.NoShine) {
                //var reflected = Tuple4.Subtract(Tuple4.Multiply(2*Tuple4.Dot(surfaceNormal, lightDirection), surfaceNormal), lightDirection);
                var reflected = Tuple4.Reflect(Tuple4.Negate(lightDirection), surfaceNormal);
                var r_dot_v = Tuple4.DotProduct(reflected, Tuple4.Negate(dir));
                if (r_dot_v > 0)
                {
                    result += intensity * Math.Pow(r_dot_v/(reflected.Length()*dir.Length()), shine);
                }
            }

            var cosine = Tuple4.DotProduct(lightDirection, surfaceNormal);
            if (cosine >= 0)
            {
                result += cosine * intensity;
            }

            return result;
        }

    }

}

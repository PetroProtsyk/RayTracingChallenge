using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public class DirectionLight : ILight
    {
        private readonly Vector3 lightDirection;

        private readonly float intensity;

        public DirectionLight(Vector3 lightDirection, float intensity)
        {
            this.lightDirection = Vector3.Normalize(lightDirection);
            this.intensity = intensity / 100f;
        }

        public Vector3 GetLightDirection(Vector3 from)
        {
            return Vector3.Normalize(lightDirection);
        }

        public float GetLightDistance(Vector3 from)
        {
            return float.MaxValue;
        }

        public float GetIntensity(Vector3 dir, int shine, Vector3 pointOnSurface, Vector3 surfaceNormal)
        {
            var result = 0.0f;

            if (shine != MaterialConstants.NoShine) {
                //var reflected = Vector3.Subtract(Vector3.Multiply(2*Vector3.Dot(surfaceNormal, lightDirection), surfaceNormal), lightDirection);
                var reflected = Vector3.Reflect(Vector3.Negate(lightDirection), surfaceNormal);
                var r_dot_v = Vector3.Dot(reflected, Vector3.Negate(dir));
                if (r_dot_v > 0)
                {
                    result += (float)(intensity*Math.Pow(r_dot_v/(reflected.Length()*dir.Length()), (double)shine));
                }
            }

            var cosine = Vector3.Dot(lightDirection, surfaceNormal);
            if (cosine >= 0)
            {
                result += cosine * intensity;
            }

            return result;
        }

    }

}

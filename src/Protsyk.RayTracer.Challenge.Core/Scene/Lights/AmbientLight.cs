using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public class AmbientLight : ILight
    {
        private readonly float intensity;

        public AmbientLight(float intensity)
        {
            this.intensity = intensity;
        }

        public Vector3 GetLightDirection(Vector3 from)
        {
            return Vector3.Zero;
        }

        public float GetLightDistance(Vector3 from)
        {
            return 0;
        }

        public float GetIntensity(Vector3 dir, int shine, Vector3 pointOnSurface, Vector3 surfaceNormal)
        {
            return intensity / 100f;
        }

    }

}

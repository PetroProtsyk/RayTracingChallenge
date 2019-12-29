using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ILight
    {
        Vector3 GetLightDirection(Vector3 from);

        float GetLightDistance(Vector3 from);

        float GetIntensity(Vector3 dir, int shine, Vector3 pointOnSurface, Vector3 surfaceNormal);
    }
}

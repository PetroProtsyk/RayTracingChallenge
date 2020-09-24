using System;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ILight
    {
        Tuple4 GetLightDirection(Tuple4 from);

        double GetLightDistance(Tuple4 from);

        double GetIntensity(Tuple4 dir, double shine, Tuple4 pointOnSurface, Tuple4 surfaceNormal);
    }
}

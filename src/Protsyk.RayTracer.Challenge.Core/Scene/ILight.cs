using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ILight
    {
        Tuple4 GetLightDirection(Tuple4 from);

        double GetLightDistance(Tuple4 from);

        Tuple4 GetShadedColor(IMaterial material, Tuple4 dir, Tuple4 pointOnSurface, Tuple4 surfaceNormal);
    }
}

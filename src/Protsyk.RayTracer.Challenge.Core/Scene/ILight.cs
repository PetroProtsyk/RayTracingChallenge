using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ILight
    {
        Tuple4 GetLightDirection(Tuple4 from);

        double GetLightDistance(Tuple4 from);

        /// <summary>
        /// Get Shaded Color
        /// </summary>
        /// <param name="objectPoint">Point in the object space that will be used with material</param>
        /// <param name="material">Material</param>
        /// <param name="eyeVector"></param>
        /// <param name="pointOnSurface"></param>
        /// <param name="surfaceNormal"></param>
        /// <returns></returns>
        Tuple4 GetShadedColor(Tuple4 objectPoint, IMaterial material, Tuple4 eyeVector, Tuple4 pointOnSurface, Tuple4 surfaceNormal);
    }
}

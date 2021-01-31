using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public class SpotLight : ILight
    {
        private readonly Tuple4 location;

        private readonly double intensity;

        private readonly ColorModel colors;

        public SpotLight(ColorModel colors, Tuple4 location, double intensity)
        {
            this.location = location;
            this.intensity = intensity;
            this.colors = colors;
        }

        public override bool Equals(object obj)
        {
            return obj is SpotLight light &&
                   location.Equals(light.location) &&
                   intensity == light.intensity &&
                   colors == light.colors;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(location.GetHashCode(), intensity, colors);
        }

        public Tuple4 GetLightDirection(Tuple4 from)
        {
            return Tuple4.Normalize(Tuple4.Subtract(location, from));
        }

        public double GetLightDistance(Tuple4 from)
        {
            return Tuple4.Subtract(location, from).Length();
        }

        public Tuple4 GetShadedColor(IMaterial material, Tuple4 dir, Tuple4 pointOnSurface, Tuple4 surfaceNormal)
        {
            var lightDirection = Tuple4.Normalize(Tuple4.Subtract(location, pointOnSurface));
            return DirectionLightCommon.GetShadedColor(material, colors.White, lightDirection, intensity, dir, pointOnSurface, surfaceNormal);
        }

    }

}

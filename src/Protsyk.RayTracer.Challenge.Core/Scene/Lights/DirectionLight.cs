using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public class DirectionLight : ILight
    {
        private readonly Tuple4 lightDirection;

        private readonly double intensity;

        private readonly ColorModel colors;

        public DirectionLight(ColorModel colors, Tuple4 lightDirection, double intensity)
        {
            this.lightDirection = Tuple4.Normalize(lightDirection);
            this.intensity = intensity;
            this.colors = colors;
        }

        public Tuple4 GetLightDirection(Tuple4 from)
        {
            return lightDirection;
        }

        public double GetLightDistance(Tuple4 from)
        {
            return double.MaxValue;
        }

        public Tuple4 GetShadedColor(IMaterial material, Tuple4 eyeVector, Tuple4 pointOnSurface, Tuple4 surfaceNormal)
        {
            return DirectionLightCommon.GetShadedColor(material, colors.White, lightDirection, intensity, eyeVector, pointOnSurface, surfaceNormal);
        }
    }

}

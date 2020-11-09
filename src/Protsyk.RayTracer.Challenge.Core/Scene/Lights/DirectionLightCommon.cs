using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public static class DirectionLightCommon
    {
        public static Lighting GetIntensity(IMaterial material, Tuple4 lightDirection, double intensity, Tuple4 dir, Tuple4 pointOnSurface, Tuple4 surfaceNormal)
        {
            var ambient = material.Ambient;
            var diffuse = 0.0;
            var specular = 0.0;

            var cosine = Tuple4.DotProduct(lightDirection, surfaceNormal);
            if (cosine >= 0)
            {
                diffuse = cosine * material.Diffuse;

                if (material.IsShining())
                {
                    //var reflected = Tuple4.Subtract(Tuple4.Multiply(2*Tuple4.Dot(surfaceNormal, lightDirection), surfaceNormal), lightDirection);
                    var reflected = Tuple4.Reflect(Tuple4.Negate(lightDirection), surfaceNormal);
                    var reflectedDotDir = Tuple4.DotProduct(reflected, Tuple4.Negate(dir));
                    if (reflectedDotDir > 0)
                    {
                        specular = material.Specular * Math.Pow(reflectedDotDir / (reflected.Length() * dir.Length()), material.Shininess);
                    }
                }
            }

            return (ambient, diffuse, specular, intensity);
        }

        internal static Tuple4 GetShadedColor(IMaterial material, object white, Tuple4 lightDirection, double intensity, Tuple4 dir, Tuple4 pointOnSurface, Tuple4 surfaceNormal)
        {
            throw new NotImplementedException();
        }

        public static Tuple4 GetShadedColor(IMaterial material, Tuple4 white, Tuple4 lightDirection, double intensity, Tuple4 dir, Tuple4 pointOnSurface, Tuple4 surfaceNormal)
        {
            Tuple4 color = material.Color;
            Lighting lighting = GetIntensity(material, lightDirection, intensity, dir, pointOnSurface, surfaceNormal);

            var shadedColor = Tuple4.Add(
                Tuple4.Scale(color, lighting.intensity * (lighting.ambient + lighting.diffuse)),
                Tuple4.Scale(white, lighting.specular)
                // TODO (Original shading): Tuple4.Scale(color, lighting.intensity * lighting.specular)
            );
            return shadedColor;
        }
    }

}

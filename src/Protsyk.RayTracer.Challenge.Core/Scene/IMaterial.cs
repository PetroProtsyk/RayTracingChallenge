using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IMaterial
    {
        double Ambient { get; }

        double Diffuse { get; }

        double Specular { get; }

        double Shininess { get; }

        double Reflective { get; }
        
        double RefractiveIndex { get; }

        double Transparency { get; }

        bool IsShining();

        /// <summary>
        /// Returns material color given the point in the object space
        /// </summary>
        /// <param name="objectPoint">point in the object space</param>
        /// <returns></returns>
        Tuple4 GetColor(Tuple4 objectPoint);
    }

    public static class MaterialConstants
    {
        public static readonly int NoShine = -1;

        public static IMaterial Default = new Materials.SolidColorMaterial();

    }
}

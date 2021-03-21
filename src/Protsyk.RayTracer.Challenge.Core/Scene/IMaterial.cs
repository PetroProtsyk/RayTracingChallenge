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

        Tuple4 GetColor(Tuple4 point);
    }

    public static class MaterialConstants
    {
        public static readonly int NoShine = -1;

        public static IMaterial Default = new Materials.SolidColorMaterial();

    }
}

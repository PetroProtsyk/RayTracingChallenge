using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IMaterial
    {
        Tuple4 Color { get; }

        double Ambient { get; }

        double Diffuse { get; }

        double Specular { get; }

        double Shininess { get; }

        double Reflective { get; }

        bool IsShining();
    }

    public static class MaterialConstants
    {
        public static readonly int NoShine = -1;

        public static IMaterial Default = new Materials.SolidColorMaterial();

    }
}

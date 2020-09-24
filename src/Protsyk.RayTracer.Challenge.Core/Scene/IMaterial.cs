using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IMaterial
    {
        Tuple4 Color { get; }

        double Shininess { get; }
    }

    public static class MaterialConstants
    {
        public static readonly int NoShine = -1;

        public static IMaterial Default = new Materials.BaseMaterial();

    }
}

using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IMaterial
    {
        Tuple4 GetColor();

        int GetShine();
    }

    public static class MaterialConstants
    {
        public static readonly int NoShine = -1;

        public static IMaterial Default = new Materials.SolidColorMaterial(
                         new Tuple4(255, 255, 255, TupleFlavour.Point), 100);

    }
}

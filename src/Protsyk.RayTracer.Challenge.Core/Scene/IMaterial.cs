using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IMaterial
    {
        Vector3 GetColor();

        int GetShine();
    }

    public static class MaterialConstants
    {
        public static readonly int NoShine = -1;
    }
}

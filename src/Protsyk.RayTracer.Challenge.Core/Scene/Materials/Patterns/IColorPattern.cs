using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns
{
    public interface IColorPattern
    {
        Tuple4 GetColor(Tuple4 point);
    }
}

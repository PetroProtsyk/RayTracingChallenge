using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IFigure : ISupportsHitCheck
    {
        Tuple4 GetNormal(Tuple4 pointOnSurface);

        IMatrix Transformation { get; set; }

        IMaterial Material { get; set; }

    }
}

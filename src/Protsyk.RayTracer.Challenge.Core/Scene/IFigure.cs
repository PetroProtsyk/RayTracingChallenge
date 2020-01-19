using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IFigure : ISupportsHitCheck
    {
        Tuple4 ColorAt(HitResult hit);

        IMaterial GetMaterial();
    }
}

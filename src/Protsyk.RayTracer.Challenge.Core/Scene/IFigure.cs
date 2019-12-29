using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IFigure : ISupportsHitCheck
    {
        Vector3 ColorAt(HitResult hit);

        IMaterial GetMaterial();
    }
}

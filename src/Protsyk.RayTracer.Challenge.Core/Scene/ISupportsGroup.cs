using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ISupportsGroup
    {
        GroupFigure Parent { get; set; }
    }
}

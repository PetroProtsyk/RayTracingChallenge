using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ICompositeFigure
    {

        Tuple4 TransformWorldPointToObjectPoint(Tuple4 worldPoint);

        Tuple4 TransformObjectNormalToWorldNormal(Tuple4 normal);

        void AddInternal(IFigure figure);

        void RemoveInternal(IFigure figure);
    }
}

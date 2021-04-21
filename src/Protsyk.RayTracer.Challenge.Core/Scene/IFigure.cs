using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface IFigure : ISupportsHitCheck, ISupportsGroup
    {
        /// <summary>
        /// Get normal of at a given point on the figure's surface in the world coordinates
        /// </summary>
        /// <param name="figure">For composite figures (i.e. groups), should be the child figure</param>
        /// <param name="pointOnSurface">Point of the figure's surface</param>
        /// <returns></returns>
        Tuple4 GetNormal(IFigure figure, Tuple4 pointOnSurface);

        Intersection[] GetIntersections(Ray ray);

        IMatrix Transformation { get; set; }

        IMaterial Material { get; set; }

    }
}

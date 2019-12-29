using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public struct HitResult
    {
        public static readonly HitResult NoHit = new HitResult(false,
                                                               null,
                                                               -1,
                                                               new Vector3(0, 0, 0),
                                                               new Vector3(0, 0, 0));

        public readonly bool IsHit;
        public readonly IFigure Figure;
        public readonly double Distance;
        public readonly Vector3 PointOnSurface;
        public readonly Vector3 SurfaceNormal;

        public HitResult(bool isHit, IFigure figure, double distance, Vector3 pointOnSurface, Vector3 surfaceNormal)
        {
            IsHit = isHit;
            Figure = figure;
            Distance = distance;
            PointOnSurface = pointOnSurface;
            SurfaceNormal = surfaceNormal;
        }
    }

}

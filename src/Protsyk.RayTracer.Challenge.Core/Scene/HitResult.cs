﻿using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public struct HitResult
    {
        public static readonly HitResult NoHit = new HitResult(false,
                                                               null,
                                                               -1,
                                                               new Tuple4(0, 0, 0, TupleFlavour.Point),
                                                               new Tuple4(0, 0, 0, TupleFlavour.Point),
                                                               new Tuple4(0, 0, 0, TupleFlavour.Vector),
                                                               new Tuple4(0, 0, 0, TupleFlavour.Vector),
                                                               false);

        public readonly bool IsHit;
        public readonly IFigure Figure;
        public readonly double Distance;
        public readonly Tuple4 PointOnSurface;
        public readonly Tuple4 PointOverSurface;
        public readonly Tuple4 SurfaceNormal;
        public readonly Tuple4 EyeVector;
        public readonly bool IsInside;

        public HitResult(bool isHit,
                        IFigure figure,
                        double distance,
                        Tuple4 pointOnSurface,
                        Tuple4 pointOverSurface,
                        Tuple4 surfaceNormal,
                        Tuple4 eyeVector,
                        bool isInside)
        {
            IsHit = isHit;
            Figure = figure;
            Distance = distance;
            PointOnSurface = pointOnSurface;
            PointOverSurface = pointOverSurface;
            SurfaceNormal = surfaceNormal;
            EyeVector = eyeVector;
            IsInside = isInside;
        }
    }

}

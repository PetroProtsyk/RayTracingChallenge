using System;
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
                                                               Tuple4.Point(0, 0, 0),
                                                               Tuple4.Point(0, 0, 0),
                                                               Tuple4.Point(0, 0, 0),
                                                               Tuple4.Point(0, 0, 0),
                                                               Tuple4.Vector(0, 0, 0),
                                                               Tuple4.Vector(0, 0, 0),
                                                               Tuple4.Vector(0, 0, 0),
                                                               false);

        public readonly bool IsHit;
        public readonly IFigure Figure;
        public readonly double Distance;
        public readonly Tuple4 ObjectPoint; // Point in the object space
        public readonly Tuple4 PointOnSurface;
        public readonly Tuple4 PointOverSurface;
        public readonly Tuple4 PointUnderSurface;
        public readonly Tuple4 SurfaceNormal;
        public readonly Tuple4 EyeVector;
        public readonly Tuple4 ReflectionVector;
        public readonly bool IsInside;

        public HitResult(bool isHit,
                        IFigure figure,
                        double distance,
                        Tuple4 objectPoint,
                        Tuple4 pointOnSurface,
                        Tuple4 pointOverSurface,
                        Tuple4 pointUnderSurface,
                        Tuple4 surfaceNormal,
                        Tuple4 eyeVector,
                        Tuple4 reflectionVector,
                        bool isInside)
        {
            IsHit = isHit;
            Figure = figure;
            Distance = distance;
            ObjectPoint = objectPoint;
            PointOnSurface = pointOnSurface;
            PointOverSurface = pointOverSurface;
            PointUnderSurface = pointUnderSurface;
            SurfaceNormal = surfaceNormal;
            EyeVector = eyeVector;
            ReflectionVector = reflectionVector;
            IsInside = isInside;
        }
    }

}

﻿using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public abstract class BaseFigure : IFigure
    {
        private IMatrix transformation;

        private IMatrix inverseTransformation;

        private IMatrix inverseTransposeTransformation;

        public IMaterial Material
        {
            get;
            set;
        }

        public IMatrix Transformation
        {
            get
            {
                return transformation;
            }
            set
            {
                transformation = value;
                inverseTransformation = null;
                inverseTransposeTransformation = null;
                if (value != null)
                {
                    inverseTransformation = MatrixOperations.Invert(value);
                    inverseTransposeTransformation = MatrixOperations.Transpose(inverseTransformation, false);
                }
            }
        }

        // This method should return shape's normal at a given point on the surface
        // The returned normal does not have to be normalized
        protected abstract Tuple4 GetBaseNormal(Tuple4 pointOnSurface);

        public Tuple4 GetNormal(Tuple4 point)
        {
            return GetTransformedNormal(point).normal;
        }

        protected (Tuple4 normal, Tuple4 objectPoint) GetTransformedNormal(Tuple4 point)
        {
            var objectPoint = point;
            if (Transformation != null)
            {
                objectPoint = TransformWorldPointToObjectPoint(point);
            }

            var normal = GetBaseNormal(objectPoint);

            if (Transformation != null)
            {
                normal = MatrixOperations.Geometry3D.Transform(inverseTransposeTransformation, normal);
                if (normal.W != 0.0)
                {
                    normal = new Tuple4(normal.X, normal.Y, normal.Z, TupleFlavour.Vector);
                }
            }

            return (Tuple4.Normalize(normal), objectPoint);
        }

        protected Tuple4 TransformWorldPointToObjectPoint(Tuple4 worldPoint)
        {
            return MatrixOperations.Geometry3D.Transform(inverseTransformation, worldPoint);
        }

        protected abstract double[] GetBaseIntersections(Ray ray);

        public double[] GetIntersections(Ray ray)
        {
            if (Transformation != null)
            {
                ray = ray.Transform(inverseTransformation);
            }

            // This used to be called with normalized ray direction, but not anymore.
            // var result = GetBaseIntersections(new Ray(ray.origin, Tuple4.Normalize(ray.dir)));
            var result = GetBaseIntersections(ray);

            if (Transformation != null)
            {
                if (result != null)
                {
                    var len = ray.dir.Length();
                    for (int i = 0; i < result.Length; ++i)
                    {
                        result[i] /= len;
                    }
                }
            }

            return result;
        }

        public virtual HitResult Hit(Tuple4 origin, Tuple4 dir)
        {
            return ClosestPositiveHit(AllHits(origin, dir));
        }

        public virtual HitResult[] AllHits(Tuple4 origin, Tuple4 dir)
        {
            var intersections = GetIntersections(new Ray(origin, dir));
            if (intersections == null)
            {
                return new HitResult[] { HitResult.NoHit };
            }

            var result = new HitResult[intersections.Length];
            for(int i=0; i<intersections.Length; ++i)
            {
                var distance = intersections[i];
                var pointOnSurface = Tuple4.Geometry3D.MovePoint(origin, dir, distance); // orig + dir*dist
                (var surfaceNormal, var objectPoint) = GetTransformedNormal(pointOnSurface);
                var eyeVector = Tuple4.Negate(dir);
                var isInside = false;
                if (Tuple4.DotProduct(surfaceNormal, eyeVector) < 0)
                {
                    isInside = true;
                    surfaceNormal = Tuple4.Negate(surfaceNormal);
                }
                var pointOverSurface = Tuple4.Add(pointOnSurface, Tuple4.Scale(surfaceNormal, Constants.Epsilon));
                result[i] = new HitResult(true, this, distance, objectPoint, pointOnSurface, pointOverSurface, surfaceNormal, eyeVector, isInside);
            }

            return result;
        }

        public static HitResult ClosestPositiveHit(HitResult[] hits)
        {
            var result = HitResult.NoHit;
            foreach (var hit in hits)
            {
                if (hit.Distance > 0.0)
                {
                    if (result.Equals(HitResult.NoHit))
                    {
                        result = hit;
                    }
                    else if (result.Distance > hit.Distance)
                    {
                        result = hit;
                    }
                }
            }
            return result;
        }
    }

}

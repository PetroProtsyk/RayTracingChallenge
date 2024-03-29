﻿using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public abstract class BaseFigure : IFigure
    {
        private IMatrix transformation;

        private IMatrix inverseTransformation;

        private IMatrix inverseTransposeTransformation;

        private ICompositeFigure parent;

        public IMaterial Material
        {
            get;
            set;
        }

        public ICompositeFigure Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (parent == value)
                {
                    return;
                }

                if (parent != null)
                {
                    parent.RemoveInternal(this);
                }

                parent = value;

                if (parent != null)
                {
                    try
                    {
                        parent.AddInternal(this);
                    }
                    catch
                    {
                        parent = null;
                        throw;
                    }
                }
            }
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
        protected abstract Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface, double u, double v);

        public Tuple4 GetNormal(IFigure figure, Tuple4 point)
        {
            return GetNormal(figure, point, 0.0, 0.0);
        }

        public Tuple4 GetNormal(IFigure figure, Tuple4 point, double u, double v)
        {
            if (figure != null && !(figure == this || figure.Parent == this))
            {
                throw new ArgumentException($"{nameof(figure)} is not part of this figure");
            }
            return GetTransformedNormal(figure, point, u, v).normal;
        }

        protected (Tuple4 normal, Tuple4 objectPoint) GetTransformedNormal(IFigure figure, Tuple4 point, double u, double v)
        {
            var objectPoint = TransformWorldPointToObjectPoint(point);
            var normal = GetBaseNormal(figure, objectPoint, u, v);
            return (TransformObjectNormalToWorldNormal(normal), objectPoint);
        }

        public Tuple4 TransformWorldPointToObjectPoint(Tuple4 worldPoint)
        {
            var objectPoint = worldPoint;
            if (Parent != null)
            {
                objectPoint = Parent.TransformWorldPointToObjectPoint(worldPoint);
            }
            if (inverseTransformation != null)
            {
                objectPoint = MatrixOperations.Geometry3D.Transform(inverseTransformation, objectPoint);
            }
            return objectPoint;
        }

        public Tuple4 TransformObjectNormalToWorldNormal(Tuple4 normal)
        {
            if (inverseTransposeTransformation != null)
            {
                normal = MatrixOperations.Geometry3D.Transform(inverseTransposeTransformation, normal);
                if (normal.W != 0.0)
                {
                    normal = Tuple4.Vector(normal.X, normal.Y, normal.Z);
                }
            }

            normal = Tuple4.Normalize(normal);

            if (Parent != null)
            {
                normal = Parent.TransformObjectNormalToWorldNormal(normal);
            }

            return normal;
        }

        /// <summary>
        /// Get local figure intersections. Ray's direction should be normalized
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        protected abstract Intersection[] GetBaseIntersections(Ray ray);

        /// <summary>
        /// The only purpose of this method is to serve unit testing
        /// </summary>
        protected virtual Intersection[] GetBaseIntersectionsWithAnyDirection(Ray ray)
        {
            return GetBaseIntersections(new Ray(ray.origin, Tuple4.Normalize(ray.dir)));
        }

        public Intersection[] GetIntersections(Ray ray)
        {
            if (Transformation != null)
            {
                ray = ray.Transform(inverseTransformation);
            }

            var result = GetBaseIntersectionsWithAnyDirection(ray);

            if (Transformation != null)
            {
                if (result != null)
                {
                    var len = ray.dir.Length();
                    for (int i = 0; i < result.Length; ++i)
                    {
                        result[i] = new Intersection(result[i].t / len, result[i].figure, result[i].u, result[i].v);
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
                var figure = intersections[i].figure;
                var distance = intersections[i].t;
                var pointOnSurface = Tuple4.Geometry3D.MovePoint(origin, dir, distance); // orig + dir*dist
                // TODO: Remove this cast to BaseFigure
                (var surfaceNormal, var objectPoint) = ((BaseFigure)figure).GetTransformedNormal(figure, pointOnSurface, intersections[i].u, intersections[i].v);
                var eyeVector = Tuple4.Negate(dir);
                var isInside = false;
                if (Tuple4.DotProduct(surfaceNormal, eyeVector) < 0)
                {
                    isInside = true;
                    surfaceNormal = Tuple4.Negate(surfaceNormal);
                }
                var pointOverSurface = Tuple4.Add(pointOnSurface, Tuple4.Scale(surfaceNormal, Constants.Epsilon));
                var pointUnderSurface = Tuple4.Subtract(pointOnSurface, Tuple4.Scale(surfaceNormal, Constants.Epsilon));
                var reflectionVector = Tuple4.Reflect(dir, surfaceNormal);

                result[i] = new HitResult(true,
                    figure,
                    distance,
                    intersections[i].u,
                    intersections[i].v,
                    objectPoint,
                    pointOnSurface,
                    pointOverSurface,
                    pointUnderSurface,
                    surfaceNormal,
                    eyeVector,
                    reflectionVector,
                    isInside);
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

        public virtual bool Includes(IFigure child)
        {
            return ReferenceEquals(this, child);
        }
    }

}
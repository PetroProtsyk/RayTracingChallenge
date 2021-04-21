using System;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public abstract class BaseFigure : IFigure
    {
        private IMatrix transformation;

        private IMatrix inverseTransformation;

        private IMatrix inverseTransposeTransformation;

        private GroupFigure group;

        public IMaterial Material
        {
            get;
            set;
        }

        public GroupFigure Parent
        {
            get
            {
                return group;
            }
            set
            {
                if (group == value)
                {
                    return;
                }

                if (group != null)
                {
                    group.RemoveInternal(this);
                }

                group = value;

                if (group != null)
                {
                    try
                    {
                        group.AddInternal(this);
                    }
                    catch
                    {
                        group = null;
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
        protected abstract Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface);

        public Tuple4 GetNormal(IFigure figure, Tuple4 point)
        {
            if (figure != null && !(figure == this || figure.Parent == this))
            {
                throw new ArgumentException($"{nameof(figure)} is not part of this figure");
            }
            return GetTransformedNormal(figure, point).normal;
        }

        protected (Tuple4 normal, Tuple4 objectPoint) GetTransformedNormal(IFigure figure, Tuple4 point)
        {
            var objectPoint = TransformWorldPointToObjectPoint(point);
            var normal = GetBaseNormal(figure, objectPoint);
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
                        result[i] = new Intersection(result[i].t / len, result[i].figure);
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
                (var surfaceNormal, var objectPoint) = ((BaseFigure)figure).GetTransformedNormal(figure, pointOnSurface);
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
                result[i] = new HitResult(true, figure, distance, objectPoint, pointOnSurface, pointOverSurface, pointUnderSurface, surfaceNormal, eyeVector, reflectionVector, isInside);
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

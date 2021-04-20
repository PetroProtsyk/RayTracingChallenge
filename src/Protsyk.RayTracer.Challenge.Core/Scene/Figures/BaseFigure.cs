using System;

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
                    normal = Tuple4.Vector(normal.X, normal.Y, normal.Z);
                }
            }

            return (Tuple4.Normalize(normal), objectPoint);
        }

        protected Tuple4 TransformWorldPointToObjectPoint(Tuple4 worldPoint)
        {
            return MatrixOperations.Geometry3D.Transform(inverseTransformation, worldPoint);
        }

        /// <summary>
        /// Get local figure intersections. Ray's direction should be normalized
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        protected abstract double[] GetBaseIntersections(Ray ray);

        /// <summary>
        /// The only purpose of this method is to serve unit testing
        /// </summary>
        protected virtual double[] GetBaseIntersectionsWithAnyDirection(Ray ray)
        {
            return GetBaseIntersections(new Ray(ray.origin, Tuple4.Normalize(ray.dir)));
        }

        public double[] GetIntersections(Ray ray)
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
                var pointUnderSurface = Tuple4.Subtract(pointOnSurface, Tuple4.Scale(surfaceNormal, Constants.Epsilon));
                var reflectionVector = Tuple4.Reflect(dir, surfaceNormal);
                result[i] = new HitResult(true, this, distance, objectPoint, pointOnSurface, pointOverSurface, pointUnderSurface, surfaceNormal, eyeVector, reflectionVector, isInside);
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

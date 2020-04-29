using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Sphere
    {
        public Tuple4 Center { get; private set; }

        public double Radius { get; private set; }

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
                if (value != null)
                {
                    inverseTransformation = MatrixOperations.Invert(value);
                }
            }
        }

        private readonly double radius2;

        private IMatrix transformation;

        private IMatrix inverseTransformation;

        private readonly bool fromOrigin;

        public Sphere(Tuple4 center, double radius)
        {
            fromOrigin = false;
            Center = center;
            Radius = radius;
            radius2 = radius * radius;
            Transformation = null;
        }

        public Sphere(IMatrix transformation)
        {
            fromOrigin = true;
            Center = new Tuple4(0.0, 0.0, 0.0, TupleFlavour.Point);
            Radius = 1.0;
            radius2 = 1.0;
            Transformation = transformation;
        }

        public Tuple4 GetNormal(Tuple4 point)
        {
            var objectPoint = point;
            if (Transformation != null)
            {
                objectPoint = MatrixOperations.Geometry3D.Transform(inverseTransformation, point);
            }

            // hit - Center
            var normal = Tuple4.Subtract(objectPoint, Center);

            if (Transformation != null)
            {
               normal = MatrixOperations.Geometry3D.Transform(MatrixOperations.Transpose(inverseTransformation, false), normal);
               if (normal.W != 0.0)
               {
                    normal = new Tuple4(normal.X, normal.Y, normal.Z, TupleFlavour.Vector);
               }
            }

            return Tuple4.Normalize(normal);
        }

        public double[] GetIntersections(Ray ray)
        {
            if (Transformation != null)
            {
                ray = ray.Transform(inverseTransformation);
            }

            var result = fromOrigin ?
                            GetIntersectionsFromBook(ray.origin, Tuple4.Normalize(ray.dir)) :
                            GetIntersections(ray.origin, Tuple4.Normalize(ray.dir));

            if (Transformation != null)
            {
                if (result != null)
                {
                    var len = ray.dir.Length();
                    for (int i=0; i<result.Length; ++i)
                    {
                        result[i] /= len;
                    }
                }
            }

            return result;
        }

        private double[] GetIntersections(Tuple4 origin, Tuple4 dir)
        {
            if (!Constants.EpsilonCompare(1.0, dir.Length()))
            {
                throw new ArgumentException("Direction should be normalized", nameof(dir));
            }

            var l = Tuple4.Subtract(Center, origin);
            var tca = Tuple4.DotProduct(l, dir);
            var d2 = Tuple4.DotProduct(l, l) - tca * tca;
            if (d2 > radius2)
            {
                return null;
            }
            var thc = Math.Sqrt(radius2 - d2);
            var t0 = tca - thc;
            var t1 = tca + thc;
            // Ray originates inside sphere
            // When t > 0 that is intersection in the direction of the ray
            // other intersection is in the opposite direction
            return new double[] {t0, t1};
        }

        private double[] GetIntersectionsFromBook(Tuple4 origin, Tuple4 dir)
        {
            if (!Constants.EpsilonCompare(1.0, dir.Length()))
            {
                throw new ArgumentException("Direction should be normalized", nameof(dir));
            }

            var sphereToRay = Tuple4.Subtract(origin, Tuple4.ZeroPoint);
            var a = Tuple4.DotProduct(dir, dir);
            var b = 2 * Tuple4.DotProduct(dir, sphereToRay);
            var c = Tuple4.DotProduct(sphereToRay, sphereToRay) - 1.0;
            var discriminant = b*b - 4 * a * c;
            if (discriminant < 0.0)
            {
                return null;
            }
            var discriminantSqrt = Math.Sqrt(discriminant);
            var t0 = (-b - discriminantSqrt) / (2 * a);
            var t1 = (-b + discriminantSqrt) / (2 * a);
            // Ray originates inside sphere
            // When t > 0 that is intersection in the direction of the ray
            // other intersection is in the opposite direction
            return new double[] {t0, t1};
        }
    }
}

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

        private double radius2;

        private IMatrix transformation;

        private IMatrix inverseTransformation;

        public Sphere(Tuple4 center, double radius)
        {
            Center = center;
            Radius = radius;
            radius2 = radius * radius;
            Transformation = MatrixOperations.Identity(4);
        }

        public Tuple4 GetNormal(Tuple4 point)
        {
            var objectPoint = point;
            if (Transformation != null)
            {
                objectPoint = MatrixOperations.Geometry3D.Transform(inverseTransformation, point);
            }

            var normal = Tuple4.Subtract(objectPoint, Center);

            if (Transformation != null)
            {
               normal = MatrixOperations.Geometry3D.Transform(MatrixOperations.Transpose(inverseTransformation), normal);
               if (normal.W != 0.0)
               {
                    normal = new Tuple4(normal.X, normal.Y, normal.Z, TupleFlavour.Vector);
               }
            }

            return Tuple4.Normalize(normal);
        }

        public double Intersects(Tuple4 origin, Tuple4 dir)
        {
            var result = GetIntersections(origin, dir);
            if (result == null)
            {
                return -1.0;
            }
            if (result[0] < 0)
            {
                return result[1];
            }
            return result[0];
        }

        public double[] GetIntersections(Ray ray)
        {
            if (Transformation != null)
            {
                ray = ray.Transform(inverseTransformation);
            }

            var result = GetIntersections(ray.origin, Tuple4.Normalize(ray.dir));

            if (Transformation != null)
            {
                if (result != null)
                {
                    for (int i=0; i<result.Length; ++i)
                    {
                        result[i] /= ray.dir.Length();
                    }
                }
            }

            return result;
        }

        public double[] GetIntersections(Tuple4 origin, Tuple4 dir)
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
            if (t0 < t1)
            {
                return new double[] {t0, t1};
            }
            else
            {
                return new double[] {t1, t0};
            }
        }
    }
}

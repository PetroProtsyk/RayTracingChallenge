using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Sphere
    {
        public Tuple4 Center { get; private set; }

        public double Radius { get; private set; }

        public IMatrix Transformation { get; set; }

        private double Radius2;

        public Sphere(Tuple4 center, double radius)
        {
            Center = center;
            Radius = radius;
            Radius2 = radius * radius;
        }

        public Tuple4 GetNormal(Tuple4 point)
        {
            var normal = Tuple4.Normalize(Tuple4.Subtract(point, Center));
            return normal;
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
                var inverse   = MatrixOperations.Invert(Transformation);

                var newOrigin = MatrixOperations.Multiply(inverse, 
                                    MatrixOperations.Geometry3D.FromTuple(ray.origin));
                var newDir    = MatrixOperations.Multiply(inverse, 
                                    MatrixOperations.Geometry3D.FromTuple(ray.dir));

                var newDirTuple = MatrixOperations.Geometry3D.ToTuple(newDir);
                ray = new Ray(
                            MatrixOperations.Geometry3D.ToTuple(newOrigin),
                            Tuple4.Normalize(newDirTuple));

                
                var result = GetIntersections(ray.origin, ray.dir);
                if (result != null)
                {
                    for (int i=0; i<result.Length; ++i)
                    {
                        result[i] /= newDirTuple.Length();
                    }
                }

                return result;
            }

            return GetIntersections(ray.origin, ray.dir);
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
            if (d2 > Radius2)
            {
                return null;
            }
            var thc = Math.Sqrt(Radius2 - d2);
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

using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene.Lights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public class BaseScene
    {
        private readonly List<IFigure> figures = new List<IFigure>();

        private readonly List<ILight> lights = new List<ILight>();

        private ColorModel colors = ColorModel.WhiteRGB;

        private bool castShadows = true;

        private int recursionDepth = 4;

        public IEnumerable<IFigure> Figures => figures;

        public IEnumerable<ILight> Lights => lights;

        public BaseScene WithShadows(bool castShadows)
        {
            this.castShadows = castShadows;
            return this;
        }

        public BaseScene WithFigures(params IFigure[] figures)
        {
            this.figures.AddRange(figures);
            return this;
        }

        public BaseScene WithLights(params ILight[] lights)
        {
            if ((this.lights.Count(l => l is AmbientLight) + lights.Count(l => l is AmbientLight) > 1))
            {
                throw new InvalidOperationException("Only one AmbientLight is allowed");
            }
            this.lights.AddRange(lights);
            return this;
        }

        public BaseScene WithColorModel(ColorModel colors)
        {
            this.colors = colors;
            return this;
        }

        public BaseScene WithRecursionDepth(int value)
        {
            this.recursionDepth = value;
            return this;
        }

        public void AddLight(ILight light)
        {
            if ((light is AmbientLight) && lights.Any(l => l is AmbientLight))
            {
                throw new InvalidOperationException("Only one AmbientLight is allowed");
            }
            lights.Add(light);
        }

        public void ClearLights()
        {
            lights.Clear();
        }

        public void Add(IFigure figure)
        {
            figures.Add(figure);
        }

        public Tuple4 CastRay(Ray r)
        {
            return CastRay(r, recursionDepth);
        }

        private Tuple4 CastRay(Ray r, int remaining)
        {
            var hit = CalculateIntersection(r.origin, r.dir);
            var allHits = CalculateAllIntersectionsSorted(r.origin, r.dir);
            var refraction = Refraction.computeRefractiveIndexes(hit.Distance, allHits);
            var color = CalculateColorAt(hit, refraction.refractiveIndexEntering, refraction.refractiveIndexExiting, remaining);
            return color;
        }

        public bool IsShadowed(ILight light, Tuple4 pointOverSurface)
        {
            var lightDir = light.GetLightDirection(pointOverSurface);
            if (!lightDir.Equals(Tuple4.ZeroVector))
            {
                var lh = CalculateIntersection(Tuple4.Add(pointOverSurface, Tuple4.Scale(lightDir, Constants.Epsilon)), lightDir);
                if (lh.IsHit)
                {
                    var distance = light.GetLightDistance(pointOverSurface);
                    if (lh.Distance < distance)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsShadowed(Tuple4 pointOnSurface)
        {
            foreach (var light in lights)
            {
                if (IsShadowed(light, pointOnSurface))
                {
                    return true;
                }
            }
            return false;
        }

        public Tuple4 CalculateRefractedColorAt(HitResult hit, double refractiveIndexEntering, double refractiveIndexExiting)
        {
            return CalculateRefractedColorAt(hit, refractiveIndexEntering, refractiveIndexExiting, recursionDepth);
        }

        private Tuple4 CalculateRefractedColorAt(HitResult hit, double refractiveIndexEntering, double refractiveIndexExiting, int remaining)
        {
            if (!hit.IsHit || remaining <= 0)
            {
                return colors.Black;
            }

            var material = hit.Figure.Material;
            if (Constants.EpsilonZero(material.Transparency))
            {
                return colors.Black;
            }

            // Calculations are based on Snell's law

            var ration = refractiveIndexEntering / refractiveIndexExiting;
            var cosI = Tuple4.DotProduct(hit.EyeVector, hit.SurfaceNormal);

            // sin(t) * sin(t) = (ration * ration) * (1 - cos(I) * cos(I))
            var sinTheta2 = ration * ration * (1 - cosI * cosI);
            if (sinTheta2 > 1.0)
            {
                // Total internal reflection
                return colors.Black;
            }

            var cosTheta = Math.Sqrt(1 - sinTheta2);
            var dir = Tuple4.Subtract(Tuple4.Scale(hit.SurfaceNormal, ration * cosI - cosTheta),
                                      Tuple4.Scale(hit.EyeVector, ration));

            var refractedRay = new Ray(hit.PointUnderSurface, dir);
            var refractedColor = CastRay(refractedRay, remaining - 1);

            return Tuple4.Scale(refractedColor, material.Transparency);
        }

        public static double Schlick(HitResult hit, double refractiveIndexEntering, double refractiveIndexExiting)
        {
            // find the cosine of the angle between the eye and normal vectors
            double cosI = Tuple4.DotProduct(hit.EyeVector, hit.SurfaceNormal);

            // total internal reflection can only occur if n1 > n2
            if (refractiveIndexEntering > refractiveIndexExiting)
            {
                double ration = refractiveIndexEntering / refractiveIndexExiting;
                double sinTheta2 = ration * ration * (1.0 - cosI * cosI);
                if (sinTheta2 > 1.0)
                {
                    return 1.0;
                }

                // compute cosine of theta_t using trig identity 
                var cosTheta = Math.Sqrt(1.0 - sinTheta2);
                // when n1 > n2, use cos(theta_t) instead 
                cosI = cosTheta;
            }

            var r0 = Math.Pow((refractiveIndexEntering - refractiveIndexExiting) / (refractiveIndexEntering + refractiveIndexExiting), 2);
            return r0 + (1 - r0) * Math.Pow((1 - cosI), 5);
        }

        public Tuple4 CalculateReflectedColorAt(HitResult hit)
        {
            return CalculateReflectedColorAt(hit, recursionDepth);
        }

        private Tuple4 CalculateReflectedColorAt(HitResult hit, int remaining)
        {
            if (!hit.IsHit || remaining <= 0)
            {
                return colors.Black;
            }

            var material = hit.Figure.Material;
            if (Constants.EpsilonZero(material.Reflective))
            {
                return Tuple4.ZeroVector;
            }

            var reflectedRay = new Ray(hit.PointOverSurface, hit.ReflectionVector);
            var reflectedColor = CastRay(reflectedRay, remaining - 1);

            return Tuple4.Scale(reflectedColor, material.Reflective);
        }

        public Tuple4 CalculateColorAt(HitResult hit, double refractiveIndexEntering, double refractiveIndexExiting)
        {
            return CalculateColorAt(hit, refractiveIndexEntering, refractiveIndexExiting, recursionDepth);
        }

        private Tuple4 CalculateColorAt(HitResult hit, double refractiveIndexEntering, double refractiveIndexExiting, int remaining)
        {
            if (!hit.IsHit)
            {
                return colors.Black;
            }

            var material = hit.Figure.Material;
            var surface = Tuple4.ZeroVector;
            foreach (var light in lights)
            {
                // Shadow
                if (castShadows && IsShadowed(light, hit.PointOverSurface))
                {
                    continue;
                }
                surface = Tuple4.Add(surface, light.GetShadedColor(hit.ObjectPoint, material, hit.EyeVector, hit.PointOnSurface, hit.SurfaceNormal));
            }

            var reflected = CalculateReflectedColorAt(hit, remaining);
            var refracted = CalculateRefractedColorAt(hit, refractiveIndexEntering, refractiveIndexExiting, remaining);

            Tuple4 color;
            if (material.Reflective > 0 && material.Transparency > 0)
            {
                var reflectance = Schlick(hit, refractiveIndexEntering, refractiveIndexExiting);
                color = Tuple4.Add(surface, Tuple4.Add(Tuple4.Scale(reflected, reflectance), Tuple4.Scale(refracted, 1.0 - reflectance)));
            }
            else
            {
                color = Tuple4.Add(surface, Tuple4.Add(reflected, refracted));
            }

            return new Tuple4(Math.Min(colors.White.X, color.X),
                              Math.Min(colors.White.X, color.Y),
                              Math.Min(colors.White.X, color.Z),
                              TupleFlavour.Vector);
        }

        private HitResult CalculateIntersection(Tuple4 origin, Tuple4 dir)
        {
            var result = HitResult.NoHit;
            foreach (var figure in figures)
            {
                var hitCheck = figure.Hit(origin, dir);
                if (hitCheck.IsHit && (!result.IsHit || hitCheck.Distance < result.Distance))
                {
                    result = hitCheck;
                }
            }
            return result;
        }

        public IEnumerable<HitResult> CalculateAllIntersections(Tuple4 origin, Tuple4 dir)
        {
            foreach (var figure in figures)
            {
                var hits = figure.AllHits(origin, dir);
                foreach (var hit in hits)
                {
                    if (hit.IsHit)
                    {
                        yield return hit;
                    }
                }
            }
        }

        public IEnumerable<HitResult> CalculateAllIntersectionsSorted(Tuple4 origin, Tuple4 dir)
        {
            return CalculateAllIntersections(origin, dir).OrderBy(x => x.Distance);
        }
    }

}

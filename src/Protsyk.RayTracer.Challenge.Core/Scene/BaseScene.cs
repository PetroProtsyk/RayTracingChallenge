﻿using Protsyk.RayTracer.Challenge.Core.Geometry;
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
            var hit = CalculateIntersection(r.origin, r.dir);
            var color = CalculateColorAt(hit);
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

        public Tuple4 CalculateColorAt(HitResult hit)
        {
            if (hit.IsHit)
            {
                var material = hit.Figure.Material;
                var color = Tuple4.ZeroVector;
                foreach (var light in lights)
                {
                    // Shadow
                    if (castShadows && IsShadowed(light, hit.PointOverSurface))
                    {
                        continue;
                    }
                    color = Tuple4.Add(color, light.GetShadedColor(material, hit.EyeVector, hit.PointOnSurface, hit.SurfaceNormal));
                }

                return new Tuple4(Math.Min(colors.White.X, color.X),
                                  Math.Min(colors.White.X, color.Y),
                                  Math.Min(colors.White.X, color.Z),
                                  TupleFlavour.Vector);
            }

            return colors.Black;
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

        public IEnumerable<HitResult> CalculateAllIntersection(Tuple4 origin, Tuple4 dir)
        {
            foreach (var figure in figures)
            {
                var hits = figure.AllHits(origin, dir);
                foreach (var hit in hits)
                {
                    yield return hit;
                }
            }
        }

        public IEnumerable<HitResult> CalculateAllIntersectionSorted(Tuple4 origin, Tuple4 dir)
        {
            return CalculateAllIntersection(origin, dir).OrderBy(x => x.Distance);
        }
    }

}

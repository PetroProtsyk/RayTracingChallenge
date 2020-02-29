using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public class BaseScene
    {
        private readonly List<IFigure> figures = new List<IFigure>();

        private readonly List<ILight> lights = new List<ILight>();

        public BaseScene WithFigures(params IFigure[] figures)
        {
            this.figures.AddRange(figures);
            return this;
        }

        public BaseScene WithLights(params ILight[] lights)
        {
            this.lights.AddRange(lights);
            return this;
        }

        public void AddLight(ILight light)
        {
            lights.Add(light);
        }

        public void Add(IFigure figure)
        {
            figures.Add(figure);
        }

        public Tuple4 CastRay(Tuple4 origin, Tuple4 dir)
        {
            var hit = CalculateIntersection(origin, dir);
            var color = CalculateColorAt(hit, dir);
            return color;
        }

        private Tuple4 CalculateColorAt(HitResult hit, Tuple4 dir)
        {
            if (hit.IsHit)
            {
                var c = hit.Figure.ColorAt(hit);
                var shine = hit.Figure.GetMaterial().GetShine();
                var intens = 0.0; // Light intensity
                foreach (var light in lights)
                {
                    // Shadow
                    {
                        var lightDir = light.GetLightDirection(hit.PointOnSurface);
                        var distance = light.GetLightDistance(hit.PointOnSurface);
                        if (!lightDir.Equals(Tuple4.ZeroVector))
                        {
                            var lh = CalculateIntersection(Tuple4.Add(hit.PointOnSurface, Tuple4.Scale(lightDir, 0.01f)), lightDir);
                            if (lh.IsHit && lh.Distance < distance)
                            {
                                continue;
                            }
                        }
                    }
                    intens += light.GetIntensity(dir, shine, hit.PointOnSurface, hit.SurfaceNormal);
                }
                var a = Tuple4.Scale(c, intens);
                return new Tuple4(Math.Min(255.0, a.X),
                                  Math.Min(255.0, a.Y),
                                  Math.Min(255.0, a.Z),
                                  TupleFlavour.Point);
            }

            return new Tuple4(0.0, 0.0, 0.0, TupleFlavour.Point);
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
    }

}

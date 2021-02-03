using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;
using Protsyk.RayTracer.Challenge.Core.Scene.Lights;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public class BaseScene
    {
        private readonly List<IFigure> figures = new List<IFigure>();

        private readonly List<ILight> lights = new List<ILight>();

        private ColorModel colors = ColorModel.WhiteRGB;

        public IEnumerable<IFigure> Figures => figures;

        public IEnumerable<ILight> Lights => lights;

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

        public BaseScene WithColorModel(ColorModel colors)
        {
            this.colors = colors;
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
                var material = hit.Figure.GetMaterial();
                var color = Tuple4.ZeroVector;
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
                    color = Tuple4.Add(color, light.GetShadedColor(material, dir, hit.PointOnSurface, hit.SurfaceNormal));
                }

                return new Tuple4(Math.Min(colors.White.X, color.X),
                                  Math.Min(colors.White.X, color.Y),
                                  Math.Min(colors.White.X, color.Z),
                                  TupleFlavour.Point);
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

    }

}

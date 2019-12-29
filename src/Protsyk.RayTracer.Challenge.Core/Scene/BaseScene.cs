using System;
using System.Collections.Generic;
using System.Numerics;
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

        public Vector3 CastRay(Vector3 origin, Vector3 dir)
        {
            var hit = CalculateIntersection(origin, dir);
            var color = CalculateColorAt(hit, dir);
            return color;
        }

        private Vector3 CalculateColorAt(HitResult hit, Vector3 dir)
        {
            if (hit.IsHit)
            {
                var c = hit.Figure.ColorAt(hit);
                var shine = hit.Figure.GetMaterial().GetShine();
                var intens = 0.0f; // Light intensity
                foreach (var light in lights)
                {
                    // Shadow
                    {
                        var lightDir = light.GetLightDirection(hit.PointOnSurface);
                        var distance = light.GetLightDistance(hit.PointOnSurface);
                        if (lightDir != Vector3.Zero)
                        {
                            var lh = CalculateIntersection(Vector3.Add(hit.PointOnSurface, Vector3.Multiply(0.01f, lightDir)), lightDir);
                            if (lh.IsHit && lh.Distance < distance)
                            {
                                continue;
                            }
                        }
                    }
                    intens += light.GetIntensity(dir, shine, hit.PointOnSurface, hit.SurfaceNormal);
                }
                var a = Vector3.Multiply(intens, c);
                return new Vector3(Math.Min(255f, a.X), Math.Min(255f, a.Y), Math.Min(255f, a.Z));
            }

            return new Vector3(0, 0, 0);
        }

        private HitResult CalculateIntersection(Vector3 origin, Vector3 dir)
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

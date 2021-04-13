using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public static class Refraction
    {
        public static (double refractiveIndexEntering, double refractiveIndexExiting) computeRefractiveIndexes(double t, HitResult[] hits)
        {
            var containers = new LinkedList<IFigure>();
            var refractiveIndexEntering = 1.0;
            var refractiveIndexExiting = 1.0;
            foreach (var h in hits)
            {
                bool isHit = Core.Constants.EpsilonCompare(h.Distance, t);
                if (isHit)
                {
                    if (containers.Count > 0)
                    {
                        refractiveIndexEntering = containers.Last.Value.Material.RefractiveIndex;
                    }
                    else
                    {
                        refractiveIndexEntering = 1.0;
                    }
                }

                var existingContainer = containers.Find(h.Figure);
                if (existingContainer != null)
                {
                    containers.Remove(existingContainer);
                }
                else
                {
                    containers.AddLast(h.Figure);
                }

                if (isHit)
                {
                    if (containers.Count > 0)
                    {
                        refractiveIndexExiting = containers.Last.Value.Material.RefractiveIndex;
                    }
                    else
                    {
                        refractiveIndexExiting = 1.0;
                    }
                    break;
                }
            }

            return (refractiveIndexEntering, refractiveIndexExiting);
        }
    }
}

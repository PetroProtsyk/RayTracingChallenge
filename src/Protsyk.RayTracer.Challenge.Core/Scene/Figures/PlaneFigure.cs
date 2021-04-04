﻿using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{

    /// <summary>
    /// XZ plane.
    /// </summary>
    public class PlaneFigure : BaseFigure
    {
        public PlaneFigure(IMatrix transformation, IMaterial material)
        {
            this.Material = material;
            this.Transformation = transformation;
        }

        protected override Tuple4 GetBaseNormal(Tuple4 pointOnSurface)
        {
            return new Tuple4(0.0, 1.0, 0.0, TupleFlavour.Vector);
        }

        protected override double[] GetBaseIntersections(Ray ray)
        {
            if (Constants.EpsilonCompare(Math.Abs(ray.dir.Y), 0))
            {
                return null;
            }

            var t = -ray.origin.Y / ray.dir.Y;
            return new double[] { t };
        }

        public override bool Equals(object obj)
        {
            return obj is SphereFigure figure &&
                   EqualityComparer<IMaterial>.Default.Equals(Material, figure.Material) &&
                   EqualityComparer<IMatrix>.Default.Equals(Transformation, figure.Transformation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Material.GetHashCode(), Transformation.GetHashCode());
        }
    }

}
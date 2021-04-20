using Protsyk.RayTracer.Challenge.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    public class Intersection
    {
        public IFigure figure;
        public double t;

        public Intersection(double t, IFigure figure)
        {
            this.t = t;
            this.figure = figure;
        }
    }
}

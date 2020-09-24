using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public class SolidColorMaterial : IMaterial
    {
        public Tuple4 Color { get; private set; }

        public double Shininess { get; private set; }

        public SolidColorMaterial(Tuple4 color, double shininess)
        {
            this.Color = color;
            this.Shininess = shininess;
        }

    }
}

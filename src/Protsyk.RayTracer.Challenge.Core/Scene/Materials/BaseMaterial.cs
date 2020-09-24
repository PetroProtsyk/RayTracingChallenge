using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public class BaseMaterial : IMaterial
    {
        public Tuple4 Color { get; private set; }

        public double Ambient { get; private set; }

        public double Diffuse { get; private set; }

        public double Specular { get; private set; }

        public double Shininess { get; private set; }

        public double Reflective { get; private set; }

        public BaseMaterial()
        {
            this.Color = new Tuple4(1, 1, 1, TupleFlavour.Point);
            this.Ambient = 0.1;
            this.Diffuse = 0.9;
            this.Specular = 0.9;
            this.Shininess = 200.0;
            this.Reflective = 0.0;
        }
    }
}

using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public class SolidColorMaterial : IMaterial
    {
        private readonly Tuple4 color;

        private readonly int shine;

        public SolidColorMaterial(Tuple4 color, int shine)
        {
            this.color = color;
            this.shine = shine;
        }

        public Tuple4 GetColor()
        {
            return color;
        }

        public int GetShine()
        {
            return shine;
        }
    }
}

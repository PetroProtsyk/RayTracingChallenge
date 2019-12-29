using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public class SolidColorMaterial : IMaterial
    {
        private readonly Vector3 color;

        private readonly int shine;

        public SolidColorMaterial(Vector3 color, int shine)
        {
            this.color = color;
            this.shine = shine;
        }

        public Vector3 GetColor()
        {
            return color;
        }

        public int GetShine()
        {
            return shine;
        }
    }
}

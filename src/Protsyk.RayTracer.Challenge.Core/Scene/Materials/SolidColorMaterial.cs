using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public class SolidColorMaterial : BaseMaterial
    {
        public Tuple4 Color { get; private set; }

        public override Tuple4 GetColor(Tuple4 point)
        {
            return Color;
        }

        public SolidColorMaterial()
        {
            this.Color = new Tuple4(1, 1, 1, TupleFlavour.Vector);
        }

        public SolidColorMaterial(Tuple4 color, double ambient, double diffuse, double specular, double shininess, double reflective, double refractiveIndex, double transparency)
            : base(ambient, diffuse, specular, shininess, reflective, refractiveIndex, transparency)
        {
            Color = color;
        }

        public static SolidColorMaterial fromColorAndShininess(Tuple4 color, double shininess) 
            => new SolidColorMaterial(color, 0.0, 1.0, 1.0, shininess, 1.0, 1.0, 0.0);

        public static SolidColorMaterial fromColorShininessReflective(Tuple4 color, double shininess, double reflective) 
            => new SolidColorMaterial(color, 0.0, 1.0, 1.0, shininess, reflective, 1.0, 0.0);

        public override bool Equals(object obj)
        {
            return obj is SolidColorMaterial material &&
                   EqualityComparer<Tuple4>.Default.Equals(Color, material.Color) &&
                   base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color.GetHashCode(), base.GetHashCode());
        }
    }
}

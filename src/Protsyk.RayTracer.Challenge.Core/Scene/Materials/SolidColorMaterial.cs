using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public class SolidColorMaterial : IMaterial
    {
        public Tuple4 Color { get; private set; }

        public double Ambient { get; private set; }

        public double Diffuse { get; private set; }

        public double Specular { get; private set; }

        public double Shininess { get; private set; }

        public double Reflective { get; private set; }

        public double RefractiveIndex { get; private set; }

        public double Transparency { get; private set; }

        public SolidColorMaterial()
        {
            this.Color = new Tuple4(1, 1, 1, TupleFlavour.Vector);
            this.Ambient = 0.1;
            this.Diffuse = 0.9;
            this.Specular = 0.9;
            this.Shininess = 200.0;
            this.Reflective = 0.0;
            this.RefractiveIndex = 1.0;
            this.Transparency = 0.0;
        }

        public SolidColorMaterial(Tuple4 color, double ambient, double diffuse, double specular, double shininess, double reflective, double refractiveIndex, double transparency)
        {
            Color = color;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Shininess = shininess;
            Reflective = reflective;
            RefractiveIndex = refractiveIndex;
            Transparency = transparency;
        }

        public static SolidColorMaterial fromColorAndShininess(Tuple4 color, double shininess) 
            => new SolidColorMaterial(color, 0.0, 1.0, 1.0, shininess, 1.0, 1.0, 0.0);

        public static SolidColorMaterial fromColorShininessReflective(Tuple4 color, double shininess, double reflective) 
            => new SolidColorMaterial(color, 0.0, 1.0, 1.0, shininess, reflective, 1.0, 0.0);

        public bool IsShining()
        {
            return Shininess != MaterialConstants.NoShine;
        }

        public override bool Equals(object obj)
        {
            return obj is SolidColorMaterial material &&
                   EqualityComparer<Tuple4>.Default.Equals(Color, material.Color) &&
                   Ambient == material.Ambient &&
                   Diffuse == material.Diffuse &&
                   Specular == material.Specular &&
                   Shininess == material.Shininess &&
                   Reflective == material.Reflective &&
                   RefractiveIndex == material.RefractiveIndex &&
                   Transparency == material.Transparency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Ambient, Diffuse, Specular, Shininess, Reflective, RefractiveIndex, Transparency);
        }
    }
}

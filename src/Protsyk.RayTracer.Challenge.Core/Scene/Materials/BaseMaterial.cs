using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public abstract class BaseMaterial : IMaterial
    {
        public double Ambient { get; private set; }

        public double Diffuse { get; private set; }

        public double Specular { get; private set; }

        public double Shininess { get; private set; }

        public double Reflective { get; private set; }

        public double RefractiveIndex { get; private set; }

        public double Transparency { get; private set; }

        public abstract Tuple4 GetColor(Tuple4 objectPoint);

        public BaseMaterial()
        {
            this.Ambient = 0.1;
            this.Diffuse = 0.9;
            this.Specular = 0.9;
            this.Shininess = 200.0;
            this.Reflective = 0.0;
            this.RefractiveIndex = 1.0;
            this.Transparency = 0.0;
        }

        public BaseMaterial(double ambient, double diffuse, double specular, double shininess, double reflective, double refractiveIndex, double transparency)
        {
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Shininess = shininess;
            Reflective = reflective;
            RefractiveIndex = refractiveIndex;
            Transparency = transparency;
        }

        public bool IsShining()
        {
            return Shininess != MaterialConstants.NoShine;
        }

        public override bool Equals(object obj)
        {
            return obj is BaseMaterial material &&
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
            return HashCode.Combine(Ambient, Diffuse, Specular, Shininess, Reflective, RefractiveIndex, Transparency);
        }
    }
}

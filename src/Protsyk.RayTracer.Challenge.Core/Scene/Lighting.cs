using System;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public struct Lighting
    {
        public readonly double diffuse;
        public readonly double specular;
        public readonly double intensity;

        public Lighting(double diffuse, double specular, double intensity)
        {
            this.diffuse = diffuse;
            this.specular = specular;
            this.intensity = intensity;
        }

        public override bool Equals(object obj)
        {
            return obj is Lighting other &&
                   diffuse == other.diffuse &&
                   specular == other.specular &&
                   intensity == other.intensity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(diffuse, specular);
        }

        public void Deconstruct(out double diffuse, out double specular, out double intensity)
        {
            diffuse = this.diffuse;
            specular = this.specular;
            intensity = this.intensity;
        }

        public static implicit operator (double diffuse, double specular, double intensity)(Lighting value)
        {
            return (value.diffuse, value.specular, value.intensity);
        }

        public static implicit operator Lighting((double diffuse, double specular, double intensity) value)
        {
            return new Lighting(value.diffuse, value.specular, value.intensity);
        }
    }
}

using System;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public struct Lighting
    {
        public readonly double ambient;
        public readonly double diffuse;
        public readonly double specular;
        public readonly double intensity;

        public Lighting(double ambient, double diffuse, double specular, double intensity)
        {
            this.ambient = ambient;
            this.diffuse = diffuse;
            this.specular = specular;
            this.intensity = intensity;
        }

        public override bool Equals(object obj)
        {
            return obj is Lighting other &&
                   ambient == other.ambient &&
                   diffuse == other.diffuse &&
                   specular == other.specular &&
                   intensity == other.intensity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ambient, diffuse, specular);
        }

        public void Deconstruct(out double ambient, out double diffuse, out double specular, out double intensity)
        {
            ambient = this.ambient;
            diffuse = this.diffuse;
            specular = this.specular;
            intensity = this.intensity;
        }

        public static implicit operator (double ambient, double diffuse, double specular, double intensity)(Lighting value)
        {
            return (value.ambient, value.diffuse, value.specular, value.intensity);
        }

        public static implicit operator Lighting((double ambient, double diffuse, double specular, double intensity) value)
        {
            return new Lighting(value.ambient, value.diffuse, value.specular, value.intensity);
        }
    }
}

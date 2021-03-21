using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials
{
    public class PatternMaterial : BaseMaterial
    {
        public IColorPattern Pattern { get; private set; }

        public override Tuple4 GetColor(Tuple4 point)
        {
            return Pattern.GetColor(point);
        }

        public PatternMaterial(IColorPattern pattern, double ambient, double diffuse, double specular, double shininess, double reflective, double refractiveIndex, double transparency)
            : base(ambient, diffuse, specular, shininess, reflective, refractiveIndex, transparency)
        {
            Pattern = pattern;
        }

        public override bool Equals(object obj)
        {
            return obj is PatternMaterial material &&
                   EqualityComparer<IColorPattern>.Default.Equals(Pattern, material.Pattern) &&
                   base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pattern.GetHashCode(), base.GetHashCode());
        }
    }
}

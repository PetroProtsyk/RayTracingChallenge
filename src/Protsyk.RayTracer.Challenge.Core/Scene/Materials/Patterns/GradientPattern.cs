using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;
using System.Collections.Generic;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns
{
    public class GradientPattern : BasePattern
    {
        public Tuple4 ColorA { get; private set; }
        public Tuple4 ColorB { get; private set; }

        private readonly Tuple4 colorDelta;

        public GradientPattern(Tuple4 colorA, Tuple4 colorB)
            : this(Matrix4x4.Identity, colorA, colorB)
        {
        }

        public GradientPattern(IMatrix transformation, Tuple4 colorA, Tuple4 colorB)
            : base(transformation)
        {
            ColorA = colorA;
            ColorB = colorB;
            colorDelta = Tuple4.Subtract(ColorB, colorA);
        }

        protected override Tuple4 GetColorAtPattern(Tuple4 pointInPatternSpace)
        {
            return Tuple4.Add(ColorA, Tuple4.Scale(colorDelta, Math.Abs(pointInPatternSpace.X) - Math.Floor(Math.Abs(pointInPatternSpace.X))));
        }

        public override bool Equals(object obj)
        {
            return obj is GradientPattern pattern && base.Equals(obj) &&
                   EqualityComparer<Tuple4>.Default.Equals(ColorA, pattern.ColorA) &&
                   EqualityComparer<Tuple4>.Default.Equals(ColorB, pattern.ColorB);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), ColorA, ColorB);
        }
    }
}

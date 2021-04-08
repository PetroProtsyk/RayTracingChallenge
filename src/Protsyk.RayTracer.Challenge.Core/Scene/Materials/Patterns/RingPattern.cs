using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;
using System.Collections.Generic;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns
{
    public class RingPattern : BasePattern
    {
        public Tuple4 ColorA { get; private set; }
        public Tuple4 ColorB { get; private set; }

        public RingPattern(Tuple4 colorA, Tuple4 colorB)
            : this(Matrix4x4.Identity, colorA, colorB)
        {
        }

        public RingPattern(IMatrix transformation, Tuple4 colorA, Tuple4 colorB)
            : base(transformation)
        {
            ColorA = colorA;
            ColorB = colorB;
        }

        protected override Tuple4 GetColorAtPattern(Tuple4 pointInPatternSpace)
        {
            if (Math.Floor(pointInPatternSpace.X * pointInPatternSpace.X + pointInPatternSpace.Z * pointInPatternSpace.Z) % 2 == 0)
            {
                return ColorA;
            }
            return ColorB;
        }

        public override bool Equals(object obj)
        {
            return obj is RingPattern pattern && base.Equals(obj) &&
                   EqualityComparer<Tuple4>.Default.Equals(ColorA, pattern.ColorA) &&
                   EqualityComparer<Tuple4>.Default.Equals(ColorB, pattern.ColorB);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), ColorA, ColorB);
        }
    }
}

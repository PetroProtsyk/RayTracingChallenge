using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns
{
    public class StripePattern : IColorPattern
    {
        public Tuple4 ColorA { get; private set; }
        public Tuple4 ColorB { get; private set; }

        public StripePattern(Tuple4 colorA, Tuple4 colorB)
        {
            this.ColorA = colorA;
            this.ColorB = colorB;
        }

        public Tuple4 GetColor(Tuple4 point)
        {
            if (Math.Floor(point.X) % 2 == 0)
            {
                return ColorA;
            }
            return ColorB;
        }

        public override bool Equals(object obj)
        {
            return obj is StripePattern pattern &&
                   EqualityComparer<Tuple4>.Default.Equals(ColorA, pattern.ColorA) &&
                   EqualityComparer<Tuple4>.Default.Equals(ColorB, pattern.ColorB);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ColorA, ColorB);
        }
    }
}

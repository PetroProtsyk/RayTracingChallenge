using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns
{
    public abstract class BasePattern : IColorPattern
    {
        private IMatrix transformation;
        private IMatrix inverseTransformation;

        public IMatrix Transformation
        {
            get
            {
                return transformation;
            }
            set
            {
                transformation = value;
                inverseTransformation = null;
                if (value != null)
                {
                    inverseTransformation = MatrixOperations.Invert(value);
                }
            }
        }

        public BasePattern(IMatrix transformation)
        {
            this.Transformation = transformation;
        }

        public Tuple4 GetColor(Tuple4 point)
        {
            if (transformation != null && transformation != Matrix4x4.Identity)
            {
                point = MatrixOperations.Geometry3D.Transform(inverseTransformation, point);
            }

            return GetColorAtPattern(point);
        }

        protected abstract Tuple4 GetColorAtPattern(Tuple4 pointInPatternSpace);

        public override bool Equals(object obj)
        {
            return obj is BasePattern pattern &&
                   EqualityComparer<IMatrix>.Default.Equals(Transformation, pattern.Transformation);
        }

        public override int GetHashCode()
        {
            return Transformation.GetHashCode();
        }
    }
}

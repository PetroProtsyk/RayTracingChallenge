using System;
using System.Linq;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public class CSGFigure : BaseFigure, ICompositeFigure
    {
        private IFigure left;
        private IFigure right;
        private readonly string op;

        public IReadOnlyCollection<IFigure> Figures => new IFigure[] { left, right };

        public IFigure Left => left;
        public IFigure Right => right;
        public string Operator => op;

        public CSGFigure(string op, IFigure left, IFigure right)
        {
            this.op = op;
            left.Parent = this;
            right.Parent = this;
        }

        protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface)
        {
            throw new InvalidOperationException("This method should never be called");
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            return null;
        }

        void ICompositeFigure.AddInternal(IFigure figure)
        {
            if (left == figure || right == figure)
            {
                return;
            }
            if (left == null)
            {
                left = figure;
            }
            else if (right == null)
            {
                right = figure;
            }
            else
            {
                throw new InvalidOperationException("Unexpected");
            }
        }

        void ICompositeFigure.RemoveInternal(IFigure figure)
        {
            if (left == figure)
            {
                left = null;
            }
            else if (right == figure)
            {
                right = null;
            }
            else
            {
                throw new InvalidOperationException("Unexpected");
            }
        }
    }

}

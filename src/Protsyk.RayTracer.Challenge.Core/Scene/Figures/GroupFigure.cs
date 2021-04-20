using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public class GroupFigure : BaseFigure
    {
        private readonly List<IFigure> figures = new List<IFigure>();

        public IReadOnlyCollection<IFigure> Figures => figures.AsReadOnly();

        public GroupFigure()
        {
        }

        protected override Tuple4 GetBaseNormal(Tuple4 pointOnSurface)
        {
            return Tuple4.ZeroPoint;
        }

        protected override double[] GetBaseIntersections(Ray ray)
        {
            return null;
        }

        public void Add(IFigure figure)
        {
            if (figure.Parent == this)
            {
                return;
            }

            figure.Parent = this;
        }

        public void Remove(IFigure figure)
        {
            if (figure.Parent != this)
            {
                throw new InvalidOperationException("Figure belongs to another group");
            }

            figure.Parent = null;
        }
        
        internal void AddInternal(IFigure figure)
        {
            figures.Add(figure);
        }

        internal void RemoveInternal(IFigure figure)
        {
            figures.Remove(figure);
        }
    }

}

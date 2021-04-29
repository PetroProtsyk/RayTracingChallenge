using System;
using System.Linq;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public class GroupFigure : BaseFigure, ICompositeFigure
    {
        private readonly List<IFigure> figures = new List<IFigure>();

        public IReadOnlyCollection<IFigure> Figures => figures.AsReadOnly();

        public GroupFigure()
            : this(Matrix4x4.Identity)
        {
        }

        public GroupFigure(IMatrix transform)
        {
            this.Transformation = transform;
        }

        protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface)
        {
            throw new InvalidOperationException("This method should never be called");
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            var result = new List<Intersection>();
            foreach (var figure in figures)
            {
                var figureIntersections = figure.GetIntersections(ray);
                if (figureIntersections != null)
                {
                    result.AddRange(figureIntersections);
                }
            }
            return result.Count == 0 ? null : result.OrderBy(x => x.t).ToArray();
        }

        public override bool Includes(IFigure child)
        {
            return figures.Any(f => f.Includes(child));
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
        
        void ICompositeFigure.AddInternal(IFigure figure)
        {
            figures.Add(figure);
        }

        void ICompositeFigure.RemoveInternal(IFigure figure)
        {
            figures.Remove(figure);
        }
    }

}

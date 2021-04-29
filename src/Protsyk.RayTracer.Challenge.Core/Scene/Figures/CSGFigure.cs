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
            : this(null, op, left, right)
        { }

        public CSGFigure(IMatrix transformation, string op, IFigure left, IFigure right)
        {
            this.Transformation = transformation;
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
            var leftIntersections = left.GetIntersections(ray);
            var rightIntersections = right.GetIntersections(ray);
            var allIntersections = SharedUtils.JoinArrays(leftIntersections, rightIntersections);
            if (allIntersections == null)
            {
                return null;
            }

            Array.Sort(allIntersections, (a, b) =>
            {
                if (a.t != b.t)
                {
                    return Comparer<double>.Default.Compare(a.t, b.t);
                }

                return Comparer<IFigure>.Default.Compare(a.figure, b.figure);
            });

            return FilterIntersections(allIntersections);
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

        public static bool IntersectionAllowed(string op, bool lhit, bool inl, bool inr)
        {
            return op switch
            {
                "union" => (lhit && !inr) || (!lhit && !inl),
                "intersection" => (lhit && inr) || (!lhit && inl),
                "difference" => (lhit && !inr) || (!lhit && inl),
                _ => throw new NotSupportedException($"Operation {op} is not supported"),
            };
        }

        public Intersection[] FilterIntersections(Intersection[] xs)
        {
            var result = new List<Intersection>(xs.Length / 2);

            // Begin outside of both children
            bool inl = false;
            bool inr = false;

            foreach (var i in xs)
            {
                bool lhit = left.Includes(i.figure);
                if (IntersectionAllowed(op, lhit, inl, inr))
                {
                    result.Add(i);
                }
                if (lhit)
                {
                    inl = !inl;
                }
                else
                {
                    inr = !inr;
                }
            }

            return result.ToArray();
        }

        public override bool Includes(IFigure child)
        {
            return left.Includes(child) || right.Includes(child);
        }
    }

}

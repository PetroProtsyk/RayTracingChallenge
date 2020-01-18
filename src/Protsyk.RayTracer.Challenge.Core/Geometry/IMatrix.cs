using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public interface IMatrix : IEquatable<IMatrix>
    {
        int Columns { get; }

        int Rows { get; }

        double this[int row, int column] { get; }
    }
}

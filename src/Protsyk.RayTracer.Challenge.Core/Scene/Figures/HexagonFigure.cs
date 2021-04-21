using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public static class HexagonFigure
    {
        private static IFigure CreateHexagonCorner(IMaterial material)
        {
            return new SphereFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(0, 0, -1),
                                                              MatrixOperations.Geometry3D.Scale(0.25, 0.25, 0.25)), material);
        }

        private static IFigure CreateHexagonEdge(IMaterial material)
        {
            return new CylinderFigure(MatrixOperations.Multiply(MatrixOperations.Geometry3D.Translation(0, 0, -1),
                                      MatrixOperations.Multiply(MatrixOperations.Geometry3D.RotateY(-Math.PI / 6.0),
                                      MatrixOperations.Multiply(MatrixOperations.Geometry3D.RotateZ(-Math.PI / 2.0),
                                                                MatrixOperations.Geometry3D.Scale(0.25, 1, 0.25)))), material, 0, 1, false);
        }

        private static IFigure CreateHexagonSide(IMatrix transformation, IMaterial material)
        {
            var side = new GroupFigure(transformation);
            side.Add(CreateHexagonCorner(material));
            side.Add(CreateHexagonEdge(material));
            return side;
        }

        public static IFigure CreateHexagon(IMatrix transformation, IMaterial material)
        {
            var hex = new GroupFigure(transformation);
            for (int n = 0; n < 6; ++n)
            {
                hex.Add(CreateHexagonSide(MatrixOperations.Geometry3D.RotateY(n * Math.PI / 3.0), material));
            }
            return hex;
        }
    }
}

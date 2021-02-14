using System;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    // This camera points at negative Z axis direction
    // and is based on formulas from the book
    public class FovCamera2 : ICamera
    {
        public double ScreenWidth {get; private set;}
        public double ScreenHeight {get; private set;}
        public Tuple4 Origin {get; private set;}
        public double FieldOfView {get; private set;}
        public double PixleSize { get; private set; }
        // View transformation
        public IMatrix Transformation
        {
            get
            {
                return transform;
            }
            set
            {
                if (transform == value)
                {
                    return;
                }
                this.transform = value;
                this.inverseTransform = MatrixOperations.Invert(value);
                this.Origin = MatrixOperations.Geometry3D.Transform(inverseTransform, Tuple4.ZeroPoint);
            }
        }

        private readonly double tangy;
        private readonly double tangx;
        private readonly double aspect;

        private IMatrix transform;
        private IMatrix inverseTransform;

        public FovCamera2(IMatrix transformation, double fieldOfView, double screenWidth, double screenHeight)
        {
            this.Transformation = transformation;
            this.FieldOfView = fieldOfView;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;

            aspect = screenWidth / screenHeight;
            var halfView = Math.Tan(fieldOfView / 2.0);

            if (Constants.EpsilonCompare(aspect, 1.0) || aspect > 1.0)
            {
                tangy = halfView / aspect;
                tangx = halfView;
            }
            else
            {
                tangy = halfView;
                tangx = halfView * aspect;
            }

            PixleSize = tangx * 2 / screenWidth;
        }

        public Ray GetRay(double screenX, double screenY)
        {
            var xoffset = (screenX + 0.5) * PixleSize;
            var yoffset = (screenY + 0.5) * PixleSize;

            var worldX = tangx - xoffset;
            var worldY = tangy - yoffset;

            var pixel = MatrixOperations.Geometry3D.Transform(inverseTransform, new Tuple4(worldX, worldY, -1.0, TupleFlavour.Point));

            var direction = Tuple4.Normalize(Tuple4.Subtract(pixel, Origin));
            return new Ray(Origin, direction);
        }
    }
}

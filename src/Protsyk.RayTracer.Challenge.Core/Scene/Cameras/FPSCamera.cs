using System;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    // This camera points at positive Z axis direction
    public class FPSCamera : ICamera
    {
        public double ScreenWidth {get; private set;}
        public double ScreenHeight {get; private set;}
        public Tuple4 Origin {get; set;}
        public double FieldOfView {get; private set;}
        public double PixleSize { get; private set; }
        public IMatrix Transformation { get; set; }

        private double yaw;
        private double pitch;

        private readonly double tangy;
        private readonly double tangx;
        private readonly double aspect;

        public FPSCamera(Tuple4 origin, double fieldOfView, double screenWidth, double screenHeight)
            : this(origin, 0, 0, fieldOfView, screenWidth, screenHeight)
        {
        }

        public FPSCamera(Tuple4 origin, double pitch, double yaw, double fieldOfView, double screenWidth, double screenHeight)
        {
            this.Origin = origin;
            this.FieldOfView = fieldOfView;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            this.yaw = yaw;
            this.pitch = pitch;
            this.Transformation = MatrixOperations.Geometry3D.EulerAnglesTransform(origin, pitch, yaw);

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
            // When j changes from  [0, height - 1],
            // y should change from [-tan(fov/2), tan(fov/2)]
            var y = -(-tangy + 2 * (screenY + 0.5) * (tangy / (ScreenHeight)));
            // When i changes from  [0, width - 1],
            // x should change from [-tan(fov/2), tan(fov/2)]
            var x = -tangx + 2 * (screenX + 0.5) * (tangx / (ScreenWidth));

            var direction = Tuple4.Normalize(
                MatrixOperations.Geometry3D.Transform(Transformation, new Tuple4(x, y, 1.0, TupleFlavour.Vector)));

            return new Ray(Origin, direction);
        }
    }
}

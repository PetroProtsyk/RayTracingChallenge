using System;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    public class FovCamera : ICamera
    {
        public double ScreenWidth {get; private set;}
        public double ScreenHeight {get; private set;}
        public Tuple4 Origin {get; private set;}
        public double FieldOfView {get; private set;}
        public double PixleSize { get; private set; }
        public IMatrix Transformation { get; private set; }

        private readonly double tangy;
        private readonly double tangx;
        private readonly double aspect;

        public FovCamera(Tuple4 origin, double fieldOfView, double screenWidth, double screenHeight)
        {
            this.Origin = origin;
            this.FieldOfView = fieldOfView;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            this.Transformation = Matrix4x4.Identity;

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
            var y = -(-tangy + 2*screenY*(tangy/(ScreenHeight-1)));
            // When i changes from  [0, width - 1],
            // x should change from [-tan(fov/2), tan(fov/2)]
            var x = -tangx + 2*screenX*(tangx/(ScreenWidth-1));

            var direction = Tuple4.Normalize(new Tuple4(x, y, 1.0, TupleFlavour.Vector));
            return new Ray(Origin, direction);
        }
    }
}

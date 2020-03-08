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

        private readonly double tangy;
        private readonly double tangx;

        public FovCamera(Tuple4 origin, double fieldOfView, double screenWidth, double screenHeight)
        {
            this.Origin = origin;
            this.FieldOfView = fieldOfView;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;

            tangy = Math.Tan(fieldOfView/2.0);
            tangx = Math.Tan(fieldOfView/2.0)*(ScreenWidth/ScreenHeight);
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

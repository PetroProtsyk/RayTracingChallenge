using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    public class FovCamera : ICamera
    {
        public float ScreenWidth {get; private set;}
        public float ScreenHeight {get; private set;}
        public Vector3 Origin {get; private set;}
        public double FieldOfView {get; private set;}

        private readonly double tangy;
        private readonly double tangx;

        public FovCamera(Vector3 origin, double fieldOfView, float screenWidth, float screenHeight)
        {
            this.Origin = origin;
            this.FieldOfView = fieldOfView;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;

            tangy = Math.Tan(fieldOfView/2.0);
            tangx = Math.Tan(fieldOfView/2.0)*((double)ScreenWidth/ScreenHeight);
        }

        public Vector3 GetDirection(float screenX, float screenY)
        {
            // When j changes from  [0, height - 1],
            // y should change from [-tan(fov/2), tan(fov/2)]
            var y = -(-tangy + 2*screenY*(tangy/(ScreenHeight-1)));
            // When i changes from  [0, width - 1],
            // x should change from [-tan(fov/2), tan(fov/2)]
            var x = -tangx + 2*screenX*(tangx/(ScreenWidth-1));

            var direction = Vector3.Normalize(new Vector3((float)x, (float)y, 1));
            return direction;
        }
    }
}

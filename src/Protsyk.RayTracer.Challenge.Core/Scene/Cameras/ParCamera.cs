
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    public class ParCamera : ICamera
    {
        public float ScreenWidth {get; private set;}
        public float ScreenHeight {get; private set;}
        public Vector3 Origin {get; private set;}
        public double ViewPortSize {get; private set;}

        private readonly double conversionY;
        private readonly double conversionX;

        public ParCamera(Vector3 origin, double viewPortSize, float screenWidth, float screenHeight)
        {
            this.Origin = origin;
            this.ViewPortSize = viewPortSize;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;

            conversionX = viewPortSize / screenWidth;
            conversionY = viewPortSize / screenHeight;
        }

        public Vector3 GetDirection(float screenX, float screenY)
        {
            var direction = Vector3.Normalize(
                    new Vector3((float)((screenX-ScreenWidth/2)*conversionX),
                                (float)((ScreenHeight/2 - screenY)*conversionY),
                                1)
                    );
            return direction;
        }
    }
}

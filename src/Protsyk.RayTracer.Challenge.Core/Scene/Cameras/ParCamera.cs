
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    public class ParCamera : ICamera
    {
        public double ScreenWidth {get; private set;}
        public double ScreenHeight {get; private set;}
        public Tuple4 Origin {get; private set;}
        public double ViewPortSize {get; private set;}

        private readonly double conversionY;
        private readonly double conversionX;

        public ParCamera(Tuple4 origin, double viewPortSize, double screenWidth, double screenHeight)
        {
            this.Origin = origin;
            this.ViewPortSize = viewPortSize;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;

            conversionX = viewPortSize / screenWidth;
            conversionY = viewPortSize / screenHeight;
        }

        public Tuple4 GetDirection(double screenX, double screenY)
        {
            var direction = Tuple4.Normalize(
                    new Tuple4((screenX - ScreenWidth/2)*conversionX,
                               (ScreenHeight/2 - screenY)*conversionY,
                               1.0,
                               TupleFlavour.Vector)
                    );
            return direction;
        }
    }
}

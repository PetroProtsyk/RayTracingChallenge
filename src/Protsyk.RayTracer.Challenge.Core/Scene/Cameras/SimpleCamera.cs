
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    //           /|
    //          / |
    //         /  |
    // -- origin  ----------->
    //         \  |          z
    //          \ |
    //           \|
    //            ^
    //         view port (origin.z + 1)
    public class SimpleCamera : ICamera
    {
        public double ScreenWidth {get; private set;}
        public double ScreenHeight {get; private set;}
        public Tuple4 Origin {get; private set;}
        public double FieldOfView => throw new NotSupportedException();
        public double PixleSize => throw new NotSupportedException();
        public IMatrix Transformation => throw new NotSupportedException();
        public double ViewPortSize {get; private set;}

        private readonly double conversionY;
        private readonly double conversionX;

        public SimpleCamera(Tuple4 origin, double viewPortSize, double screenWidth, double screenHeight)
        {
            this.Origin = origin;
            this.ViewPortSize = viewPortSize;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;

            conversionX = viewPortSize / screenWidth;
            conversionY = viewPortSize / screenHeight;
        }

        public Ray GetRay(double screenX, double screenY)
        {
            var direction = Tuple4.Normalize(
                new Tuple4((screenX - ScreenWidth/2)*conversionX,
                            (ScreenHeight/2 - screenY)*conversionY,
                            1.0,
                            TupleFlavour.Vector)
            );
            return new Ray(Origin, direction);
        }
    }
}

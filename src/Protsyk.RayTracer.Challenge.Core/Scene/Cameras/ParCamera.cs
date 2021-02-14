
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Cameras
{
    //                    ^
    //       | ---->      |
    //       | ---->      |
    // -- originZ ------- 0 ------------>
    //       | ---->      |             z
    //       | ---->      |
    //
    public class ParCamera : ICamera
    {
        public double ScreenWidth {get; private set;}
        public double ScreenHeight {get; private set;}
        public Tuple4 Origin {get; private set;}
        public double FieldOfView => throw new NotSupportedException();
        public double PixleSize => throw new NotSupportedException();
        public IMatrix Transformation
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public double ViewPortSize {get; private set;}

        private readonly double conversionY;
        private readonly double conversionX;

        private static readonly Tuple4 oz = new Tuple4(0.0, 0.0, 1.0, TupleFlavour.Vector);

        public ParCamera(double originZ, double viewPortSize, double screenWidth, double screenHeight)
        {
            this.Origin = new Tuple4(0.0, 0.0, originZ, TupleFlavour.Point);
            this.ViewPortSize = viewPortSize;
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;

            conversionX = viewPortSize / screenWidth;
            conversionY = viewPortSize / screenHeight;
        }

        public Ray GetRay(double screenX, double screenY)
        {
            var x = (screenX - ScreenWidth/2)*conversionX;
            var y = (ScreenHeight/2 - screenY)*conversionY;

            var rayOrigin = new Tuple4(x, y, Origin.Z, TupleFlavour.Point);
            return new Ray(rayOrigin, oz);
        }
    }
}

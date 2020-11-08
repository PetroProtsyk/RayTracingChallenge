using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Lights
{
    public class ColorModel
    {
        public static ColorModel WhiteNormalized = new ColorModel(1.0);

        public static ColorModel WhiteRGB = new ColorModel(255.0);

        public Tuple4 White { get; internal set; }

        public Tuple4 Black { get; internal set; }


        private ColorModel(double white)
        {
            this.White = new Tuple4(white, white, white, TupleFlavour.Vector);
            this.Black = new Tuple4(0.0, 0.0, 0.0, TupleFlavour.Vector);
        }
    }
}
using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ICamera
    {
        double ScreenWidth {get;}
        double ScreenHeight {get;}
        double FieldOfView { get; }
        double PixleSize { get; }
        IMatrix Transformation { get; set; }
        Tuple4 Origin {get;}

        Ray GetRay(double screenX, double screenY);
    }
}

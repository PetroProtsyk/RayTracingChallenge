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
        Tuple4 Origin {get;}

        Tuple4 GetDirection(double screenX, double screenY);
    }
}

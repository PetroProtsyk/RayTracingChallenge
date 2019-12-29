using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ICamera
    {
        float ScreenWidth {get;}
        float ScreenHeight {get;}
        Vector3 Origin {get;}

        Vector3 GetDirection(float screenX, float screenY);
    }
}

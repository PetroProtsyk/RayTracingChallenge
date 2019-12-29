using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ISupportsHitCheck
    {
        // If intersects, returns closest positive distance from a given origin
        // to the surface of the object in a given direction
        HitResult Hit(Vector3 origin, Vector3 dir);
    }
}

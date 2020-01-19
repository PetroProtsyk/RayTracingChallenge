using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public interface ISupportsHitCheck
    {
        // If intersects, returns closest positive distance from a given origin
        // to the surface of the object in a given direction
        HitResult Hit(Tuple4 origin, Tuple4 dir);
    }
}

using System;

namespace Protsyk.RayTracer.Challenge.Core.Canvas
{
    public interface IColorConverter<T>
    {
        Color From(T color);
    }
}
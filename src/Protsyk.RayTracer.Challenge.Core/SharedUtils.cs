using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core
{
    public static class SharedUtils
    {
        public static T[] JoinArrays<T>(T[] a, T[] b)
        {
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }

            var result = new T[a.Length + b.Length];
            Array.Copy(a, result, a.Length);
            Array.Copy(b, 0, result, a.Length, b.Length);
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityScene.Utilities
{
    public static class MathUtilities
    {

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            var tc = Math.Clamp(t,0,1);

            var x = Mathf.Lerp(a.x, b.x, tc);
            var y = Mathf.Lerp(a.y, b.y, tc);
            var z = Mathf.Lerp(a.z, b.z, tc);

            return new Vector3(x, y, z);
        }

       
    }
}

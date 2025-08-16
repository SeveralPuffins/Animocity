using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities
{
    public class BuildingComponentData_GiantGlove : BuildingComponentData
    {
        public float baseReloadTime;
        public float launchVelocity;
        public float maxDistance;

        public float BaseWindingSpeed
        {
            get
            {
                return maxDistance / baseReloadTime;
            }
        }
    }
}

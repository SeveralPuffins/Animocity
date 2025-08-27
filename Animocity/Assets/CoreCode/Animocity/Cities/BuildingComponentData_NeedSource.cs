using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities
{
    public class BuildingComponentData_NeedSource : BuildingComponentData
    {
        public ResourceBlue consumable;
        public float consumptionPerPersonPerMinute;
        public float baseQuality;
    }
}

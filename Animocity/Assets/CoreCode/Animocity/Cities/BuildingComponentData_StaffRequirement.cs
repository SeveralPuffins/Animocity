using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities
{
    public class BuildingComponentData_StaffRequirement : BuildingComponentData
    {
        public int maxStaff;
        public int minStaff;
        public float efficiencyAtMax;
        public float efficiencyAtMin;
        public int defaultPriority;

        public List<PopulationBlue> populationTypesAccepted;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_StaffRequirement : BuildingComponent
    {
        public BuildingComponent_StaffRequirement(BuildingComponentData data, Building building) : base(data, building) { }

        public BuildingComponentData_StaffRequirement StaffData
        {
            get
            {
                return Data as BuildingComponentData_StaffRequirement;
            }
        }

        protected override bool Tick(Building building)
        {
            /*string staffTypes = "";
            
            foreach(var pop in StaffData.populationTypesAccepted)
            {
                staffTypes += $"{pop.label}, ";
            }

            MonoBehaviour.print($"BUILDING {building.Blue.DisplayName} WANTS {StaffData.maxStaff} from {staffTypes}");
            */
            return base.Tick(building);
        }

    }
}

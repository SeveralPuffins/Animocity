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

        protected override bool Tick(Building building)
        {
           return base.Tick(building);
        }

    }
}

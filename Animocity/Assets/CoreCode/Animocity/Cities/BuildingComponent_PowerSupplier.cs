using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_PowerSupplier : BuildingComponent_Power
    {
        public BuildingComponent_PowerSupplier(BuildingComponentData data, Building building) : base(data, building){}

        protected override string GetInfo()
        {
            if (!IsPowered) return "Off";
            else return $"On: Producing {this.PowerData.powerAmount}MW";
        }
    }
}
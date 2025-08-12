using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_PowerConsumer : BuildingComponent_Power
    {
        public BuildingComponent_PowerConsumer(BuildingComponentData data, Building building) : base(data, building) { }

        protected override string GetInfo()
        {
            if (!IsPowered) return "Off";
            else return $"Consuming {this.PowerData.powerAmount}MW";
        }

        public override float ModifyEfficiency(float efficiency)
        {
            if (connectedGrid.HasPower && IsPowered)
            {
                return efficiency;
            }
            else
            {
                return 0f;
            }
        }

    }
}

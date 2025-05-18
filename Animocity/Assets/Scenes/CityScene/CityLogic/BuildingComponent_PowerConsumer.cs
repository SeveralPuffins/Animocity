using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_PowerConsumer : BuildingComponent_Power
    {
        public BuildingComponent_PowerConsumer(BuildingComponentData data, Building building) : base(data, building) { }

        protected override bool Tick(Building building)
        {
            //MonoBehaviour.print($"Building {building.Blue.DisplayName} demands {PowerData.powerConsumption} power!");
            this.IsPowered = 
                    connectedGrid!=null 
                    && connectedGrid.TryFindPower(this);

            return base.Tick(building);
        }

    }
}

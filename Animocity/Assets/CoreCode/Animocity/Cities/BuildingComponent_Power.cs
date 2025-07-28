using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_Power : BuildingComponent
    {
        public BuildingComponentData_Power PowerData => this.Data as BuildingComponentData_Power;

        public bool IsPowered { get; protected set; } = true;
        public PowerGrid connectedGrid {get; protected set;}

        public BuildingComponent_Power(BuildingComponentData data, Building building) : base(data, building) { }

        protected override void OnBuild()
        {
            UpdateGrid(CityInventory.Current.PowerGrid);
            base.OnBuild();
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


        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateGrid(PowerGrid newGrid)
        {
            this.connectedGrid = newGrid;
            newGrid.ConnectToGrid(this);
        }
    }
}

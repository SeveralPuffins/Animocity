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

        public bool IsPowered { get; protected set; }
        public PowerGrid connectedGrid {get; protected set;}

        public BuildingComponent_Power(BuildingComponentData data, Building building) : base(data, building) { }

        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateGrid(PowerGrid newGrid)
        {
            this.connectedGrid = newGrid;
        }
    }
}

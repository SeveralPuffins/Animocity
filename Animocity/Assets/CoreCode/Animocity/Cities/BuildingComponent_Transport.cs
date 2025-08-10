using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_Transport : BuildingComponent
    {
        public BuildingComponentData_Transport TransportData => this.Data as BuildingComponentData_Transport;

        public BuildingComponent_Transport(BuildingComponentData data, Building building) : base(data, building) { }

        public TransportGrid TransportGrid { get; private set; }

        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateGrid(TransportGrid newGrid)
        {
            this.TransportGrid = newGrid;
        }

        protected override void OnBuild()
        {
            var location = Building.GridLocation + Vector2Int.up;
            CityOverview.Current.HousingManager.AddTransport(this, this.Building.Grid, location);
            base.OnBuild();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities
{
    public class BuildingComponent_PowerSupplier : BuildingComponent_Power
    {
        public BuildingComponent_PowerSupplier(BuildingComponentData data, Building building) : base(data, building){}

        protected override bool Tick(Building building)
        {
            //connectedGrid.TryStorePower(this);
            
            connectedGrid.Resupply(this);

            return base.Tick(building);
        }
    }
}

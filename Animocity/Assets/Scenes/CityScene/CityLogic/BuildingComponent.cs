using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities
{
    public class BuildingComponent
    {
        public Building Building { get; protected set; }
        public BuildingComponentData Data
        {
            get; protected set;
        }
        public BuildingComponent(BuildingComponentData data, Building building)
        {
            Data = data;
            Building = building;
            Building.Tick += this.Tick;
            Building.LongTick += this.Tick;
        }

        protected virtual bool Tick(Building building)
        {
            return true;
        }

        protected virtual bool LongTick(Building building)
        {
            return true;
        }
    }
}

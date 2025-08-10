using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            OnBuild();
        }

        protected virtual void OnBuild()
        {
            //MonoBehaviour.print($"Initialising component {this.GetType()} on {Building.Blue.DisplayName}");
        }

        public virtual float ModifyEfficiency(float efficiency)
        {
            return efficiency;
        }

        protected virtual bool Tick(Building building)
        {
            return true;
        }

        protected virtual bool LongTick(Building building)
        {
            return true;
        }

        public virtual void AddInspectorInfo(BuildingInspectorComp inspector, bool select = false)
        {
            
        }
    }
}

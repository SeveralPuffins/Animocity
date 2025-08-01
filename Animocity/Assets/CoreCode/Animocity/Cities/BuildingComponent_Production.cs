using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_Production: BuildingComponent
    {
        public BuildingComponent_Production(BuildingComponentData data, Building building) : base(data, building)
        {
            this.selectedProcess = ProductionData.supportedProcesses.FirstOrDefault();
        }

        public BuildingComponentData_Production ProductionData
        {
            get
            {
                return Data as BuildingComponentData_Production;
            }
        }


        protected ProcessBlueprint selectedProcess;
        protected float currentProgress = 0f;

        protected override bool Tick(Building building)
        {
            
            if (CityInventory.Current.HasResources(selectedProcess.inputs))
            {
                currentProgress += this.Building.BuildingEfficiency*Building.SECONDS_PER_TICK;
                if (currentProgress >= selectedProcess.productivityCost)
                {
                    FinishProcess();
                }
            }
            

            return base.Tick(building);
        }

        protected void FinishProcess()
        {
            foreach (var key in selectedProcess.inputs.Keys)
            {
                CityInventory.Current.TakeResource(this.Building.GridLocation, key, selectedProcess.inputs[key]);
            }
            foreach (var key in selectedProcess.outputs.Keys)
            {
                CityInventory.Current.PushResource(this.Building.GridLocation, key, selectedProcess.outputs[key]);
            }
            currentProgress = 0f;
        }
    }
}

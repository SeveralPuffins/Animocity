using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_Housing : BuildingComponent
    {
        public BuildingComponentData_Housing HousingData => this.Data as BuildingComponentData_Housing;

        public PowerGrid connectedGrid {get; protected set;}

        public BuildingComponent_Housing(BuildingComponentData data, Building building) : base(data, building) { }
        
        protected override void OnBuild()
        {
            UpdateCityHousing();
            base.OnBuild();
        }

        public float CurrentSatisfaction
        {
            get
            {
                
                float currentSatisfaction = HousingData.minSatisfaction + ((HousingData.maxSatisfaction - HousingData.minSatisfaction) * this.Building.BuildingEfficiency);

                return currentSatisfaction;
            }
        }




        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateCityHousing()
        {
            CityInventory.Current.HousingManager.AddHouse(this);
        }
    }
}

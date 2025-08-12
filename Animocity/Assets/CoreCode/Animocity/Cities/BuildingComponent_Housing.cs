using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

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

        public int RoomsAvailable
        {
            get
            {
                return HousingData.capacity - NumCurrentResidents;
            }
        }

        public int NumCurrentResidents { get; private set; }

        public void AddResidents(int numResidents)
        {
            NumCurrentResidents += numResidents;
        }
        
        public void ResetResidents()
        {
            NumCurrentResidents = 0;
        }

        public float CurrentSatisfaction
        {
            get
            {
                float currentSatisfaction = HousingData.minSatisfaction + ((HousingData.maxSatisfaction - HousingData.minSatisfaction) * this.Building.BuildingEfficiency);

                return currentSatisfaction;
            }
        }

        protected override bool HasInspector() => true;
        protected override void PopulateInspectorContentPane(Transform inspectorPane)
        {
            Func<string> genText = ()=>$"{this.NumCurrentResidents}/{HousingData.capacity} residents.";
            var info = UIPrefabHelpers.Current.GetInfoBox(genText);
            info.transform.SetParent(inspectorPane);
        }




        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateCityHousing()
        {
            CityOverview.Current.HousingManager.AddHouse(this);
        }
    }
}

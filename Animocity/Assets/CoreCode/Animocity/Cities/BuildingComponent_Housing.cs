using Animocity.Cities.Algorithms;
using Animocity.UI;
using Animocity.Utilities;
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
        private List<Commute> _commutes = new List<Commute>();

        public BuildingComponent_Housing(BuildingComponentData data, Building building) : base(data, building) 
        {
            this._residents = new();
            this._commutes = new();
        }
        
        protected override void OnBuild()
        {
            UpdateCityHousing();
            base.OnBuild();
        }

        public int RoomsAvailable
        {
            get
            {
                return HousingData.capacity - NumTotalResidents;
            }
        }

        private Dictionary<PopulationBlue, int> _residents;
        public int NumTotalResidents => _residents.Values.Sum();
        public int CurrentResidents(PopulationBlue pop)
        {
            if(_residents.TryGetValue(pop, out int cr))
            {
                return cr;
            }
            return 0;
        }

        public void AddResidents(int numResidents, PopulationBlue pop)
        {
            _residents[pop] = CurrentResidents(pop)+numResidents;
        }
        
        public void ResetResidents()
        {
            _residents.Clear();
            _commutes.Clear();
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
            Func<string> genText = ()=>$"{this.NumTotalResidents}/{HousingData.capacity} residents.";
            var info = UIPrefabHelpers.Current.GetInfoBox(genText);
            info.transform.SetParent(inspectorPane);
        }

        protected override bool LongTick(Building building)
        {
            FireCommuter();
            return base.LongTick(building);
        }

        private void FireCommuter()
        {
            if(this._commutes.Count > 0)
            {
                var selected = _commutes.WeightedRandom(com => com.CommuterCount);

                CityOverview.Current.FleaCircusManager.MakeCommuter(selected);
            }
        }


        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateCityHousing()
        {
            CityOverview.Current.HousingManager.AddHouse(this);
        }

        internal void AddCommute(Commute commute)
        {
            _commutes.Add(commute);
        }
    }
}

using Animocity.Cities.Algorithms;
using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Animocity.Cities
{
    public class BuildingComponent_NeedSource : BuildingComponent
    {
        public BuildingComponent_NeedSource(BuildingComponentData data, Building building) : base(data, building) 
        {
            
        }

        // COULD BE A DICT WITH PATH IF WE WANT TO FLEA CIRCUS THIS ROUTE
        private List<BuildingComponent_Housing> _needSubscriberResidences = new();

        public bool ServiceSuspended { get; private set; } = false;
        public bool SupplyFailed { get; private set; } = false;
        public float ServiceQuality { get; private set; } = 0;

        public BuildingComponentData_NeedSource NeedData
        {
            get
            {
                return Data as BuildingComponentData_NeedSource;
            }
        }

        protected override void OnBuild()
        {
            CityOverview.Current.HousingManager.AddNeedsBuilding(this);
        }

        protected override bool Tick(Building building)
        {
            
            this.ServiceSuspended = (building.BuildingEfficiency == 0);
            if (!ServiceSuspended) 
            {
                this.ServiceQuality = NeedData.baseQuality * Building.BuildingEfficiency;

                float consumption = NeedData.consumptionPerPersonPerMinute * PeopleFed * Building.SECONDS_PER_TICK / 60f;

                if (CityOverview.Current.HasResource(NeedData.consumable, consumption))
                {
                    CityOverview.Current.TakeResource(building.GridLocation, NeedData.consumable, consumption);
                    SupplyFailed = false;
                }
                else
                {
                    SupplyFailed = true;
                }
            }
            return base.Tick(building);
        }
        protected override bool HasInspector() => true;

        protected virtual string GetInfoMsg()
        {
            string msg;
            if (ServiceSuspended)
            {
                msg = "Service Suspended";
            }
            else if (SupplyFailed)
            {
                msg = "Supply Failed!";
            }
            else
            {
                msg = $"Providing {PeopleFed * NeedData.consumptionPerPersonPerMinute} {NeedData.consumable.DisplayName} to nearby houses.";
            }
            return msg;
        }

        protected override void PopulateInspectorContentPane(Transform inspectorPane)
        {
            Func<string> genText = this.GetInfoMsg;
            var info = UIPrefabHelpers.Current.GetInfoBox(genText);
            info.transform.SetParent(inspectorPane);
        }

        public void ClearNeedSubscriberResidences()
        {
            _needSubscriberResidences.Clear();
        }

        public void AddSubscriber(BuildingComponent_Housing house)
        {
            this._needSubscriberResidences.Add(house);
        }

        public int PeopleFed
        {
            get
            {
                return _needSubscriberResidences.Sum((h)=>h.NumTotalResidents);
            }
        }
    }
}

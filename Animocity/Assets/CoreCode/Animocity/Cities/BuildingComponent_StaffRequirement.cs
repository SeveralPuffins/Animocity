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
    public class BuildingComponent_StaffRequirement : BuildingComponent
    {
        public BuildingComponent_StaffRequirement(BuildingComponentData data, Building building) : base(data, building) 
        {
            this.Priority = StaffData.defaultPriority;
            this._acceptedWorkerPops = new(StaffData.populationTypesAccepted);
        }

        private Dictionary<BuildingComponent_Housing, Path<Vector2Int>> _connectedHouses = new();

        public BuildingComponentData_StaffRequirement StaffData
        {
            get
            {
                return Data as BuildingComponentData_StaffRequirement;
            }
        }

        private List<PopulationBlue> _acceptedWorkerPops;
        public IEnumerable<PopulationBlue> AcceptedPops
        {
            get
            {
                return _acceptedWorkerPops;
            }
        }

        protected override void OnBuild()
        {
            CityOverview.Current.WorkforceManager.AddWorkplace(this);
        }

        public void ClearStaffForReassignment()
        {
            this.CurrentStaff = 0;
        }

        public void AddStaff(int numToAdd)
        {
            this.CurrentStaff += numToAdd;
        }

        public int CurrentStaff { get; private set; }

        public override float ModifyEfficiency(float efficiency)
        {
            return efficiency * Math.Max(0f, (this.CurrentStaff - StaffData.minStaff) * 1f / (1f* (StaffData.maxStaff - StaffData.minStaff)));
        }

        protected override bool HasInspector() => true;

        protected virtual string GetInfoMsg()
        {
            string msg;
            if (Priority == 0)
            {
                msg = "Work Halted";
            }
            else
            {
                msg = $"Current staff: {CurrentStaff}/{StaffData.maxStaff}";
            }
            return msg;
        }

        protected override void PopulateInspectorContentPane(Transform inspectorPane)
        {
            Func<string> genText = this.GetInfoMsg;
            var info = UIPrefabHelpers.Current.GetInfoBox(genText);
            info.transform.SetParent(inspectorPane);
            var pc = UIPrefabHelpers.Current.GetPriorityControl(this);
            pc.transform.SetParent(inspectorPane.transform);
        }

        public int Priority { get; protected set; }
        public void UpdatePriority(int priority)
        {
            this.Priority = priority;
        }

        protected override bool Tick(Building building)
        {
            return base.Tick(building);
        }

        protected override bool LongTick(Building building)
        {
            return base.LongTick(building);
        }

    }
}

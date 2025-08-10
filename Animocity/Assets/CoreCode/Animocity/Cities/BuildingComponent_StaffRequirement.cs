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
        public BuildingComponent_StaffRequirement(BuildingComponentData data, Building building) : base(data, building) { }

        private List<Commute<Vector2Int>> _commutes = new();
        private Dictionary<BuildingComponent_Housing, Path<T>> _connectedHouses = new();

        public BuildingComponentData_StaffRequirement StaffData
        {
            get
            {
                return Data as BuildingComponentData_StaffRequirement;
            }
        }

        protected override void OnBuild()
        {
            CityOverview.Current.WorkforceManager.AddWorkplace(this);
        }

        public void ClearStaffForReassignment()
        {
            this.CurrentStaff = 0;
            this._commutes.Clear();
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

        public override void AddInspectorInfo(BuildingInspectorComp inspector, bool select = false)
        {
            base.AddInspectorInfo(inspector);

            Button tabButton = UIPrefabHelpers.Current.GetInspectorButton();

            tabButton.onClick.AddListener(() => {
                inspector.ClearContentPane();
                this.populateInspectorPane(inspector.contentPane);
            });
            tabButton.transform.SetParent(inspector.tabPane);
            inspector.ClearContentPane();
            this.populateInspectorPane(inspector.contentPane);
        }

        private void populateInspectorPane(Transform contentPane)
        {
            var txt = contentPane.GetComponentInChildren<TMP_Text>();

            string msg = $"Current staff: {CurrentStaff}/{StaffData.maxStaff}";

            txt.text = msg;
        }
        protected override bool Tick(Building building)
        {
            /*string staffTypes = "";
            
            foreach(var pop in StaffData.populationTypesAccepted)
            {
                staffTypes += $"{pop.label}, ";
            }

            MonoBehaviour.print($"BUILDING {building.Blue.DisplayName} WANTS {StaffData.maxStaff} from {staffTypes}");
            */
            return base.Tick(building);
        }

        protected override bool LongTick(Building building)
        {
            CityOverview.Current.
        }

    }
}

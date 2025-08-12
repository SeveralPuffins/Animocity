using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Animocity.Cities
{
    public class BuildingComponent_Power : BuildingComponent
    {
        public BuildingComponentData_Power PowerData => this.Data as BuildingComponentData_Power;

        public bool IsPowered { get; protected set; } = true;
        public PowerGrid connectedGrid {get; protected set;}

        public BuildingComponent_Power(BuildingComponentData data, Building building) : base(data, building) { }

        protected override void OnBuild()
        {
            UpdateGrid(CityOverview.Current.PowerGrid);
            base.OnBuild();
        }
        protected override bool HasInspector() => true;

        protected override void PopulateInspectorContentPane(Transform inspectorPane)
        {
            Func<string> genText = this.GetInfo;
            var info = UIPrefabHelpers.Current.GetInfoBox(genText);
            info.transform.SetParent(inspectorPane);
        }

        protected virtual string GetInfo()
        {
            if (!IsPowered) return "Off";
            else return "";
        }


        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateGrid(PowerGrid newGrid)
        {
            this.connectedGrid = newGrid;
            newGrid.ConnectToGrid(this);
        }
    }
}

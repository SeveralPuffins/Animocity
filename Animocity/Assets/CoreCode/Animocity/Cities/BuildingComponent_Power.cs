using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        protected virtual void populateInspectorPane(Transform contentPane)
        {
            var txt = contentPane.GetComponentInChildren<TMP_Text>();
            if (!IsPowered) txt.text = "Off";
        }


        // THIS REALLY WANTS CHANGING TO A CHECK WITH THE POWER GRID MANAGER FOR WHICH GRID THE BUILDING SQUARE IS ON
        public void UpdateGrid(PowerGrid newGrid)
        {
            this.connectedGrid = newGrid;
            newGrid.ConnectToGrid(this);
        }
    }
}

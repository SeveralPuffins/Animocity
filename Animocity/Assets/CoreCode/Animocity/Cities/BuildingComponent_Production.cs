using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Animocity.Cities
{
    public class BuildingComponent_Production: BuildingComponent
    {
        public BuildingComponent_Production(BuildingComponentData data, Building building) : base(data, building)
        {
            this.SelectedProcess = ProductionData.supportedProcesses.FirstOrDefault();
        }

        public BuildingComponentData_Production ProductionData
        {
            get
            {
                return Data as BuildingComponentData_Production;
            }
        }


        public ProcessBlueprint SelectedProcess { get; protected set; }
        public float CurrentProgress { get; protected set; } = 0f;

        protected override bool Tick(Building building)
        {
            
            if (CityOverview.Current.HasResources(SelectedProcess.inputs))
            {
                CurrentProgress += this.Building.BuildingEfficiency*Building.SECONDS_PER_TICK;
                if (CurrentProgress >= SelectedProcess.productivityCost)
                {
                    FinishProcess();
                }
            }
            

            return base.Tick(building);
        }

        protected void FinishProcess()
        {
            foreach (var key in SelectedProcess.inputs.Keys)
            {
                CityOverview.Current.TakeResource(this.Building.GridLocation, key, SelectedProcess.inputs[key]);
            }
            foreach (var key in SelectedProcess.outputs.Keys)
            {
                CityOverview.Current.PushResource(this.Building.GridLocation, key, SelectedProcess.outputs[key]);
            }
            CurrentProgress = 0f;
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
            txt.text = $"Current job: {SelectedProcess.DisplayName}.";

            var panel = UIPrefabHelpers.Current.GetProductionTimerPanel(this);
            panel.transform.SetParent(contentPane);
        }
    }
}

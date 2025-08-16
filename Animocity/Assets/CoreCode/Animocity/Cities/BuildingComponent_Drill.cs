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
    public class BuildingComponent_Drill: BuildingComponent
    {
        private DrillController _drillCon;
        public BuildingComponent_Drill(BuildingComponentData data, Building building) : base(data, building)
        {
            this._drillCon = building.GetComponentInChildren<DrillController>();
            _drillCon.SetMaxExtension(DrillData.maxExtensionDist);
            UpdateParams();
        }

        public BuildingComponentData_Drill DrillData
        {
            get
            {
                return Data as BuildingComponentData_Drill;
            }
        }

        
        protected override bool Tick(Building building)
        {
            UpdateParams();

            return base.Tick(building);
        }

        private void UpdateParams()
        {

            if(this.Building.BuildingEfficiency == 0)
            {
                _drillCon.baseMotorSpeed = 0f;
                _drillCon.Powered = false;
            }
            else
            {
                _drillCon.baseMotorSpeed = DrillData.motorSpeed;
                _drillCon.Powered = true;
            }
        }

        protected override bool HasInspector() => true;

        protected override void PopulateInspectorContentPane(Transform inspectorPane)
        {
            Func<string> genText = () =>
            {
                if (!_drillCon.Powered) return "DILL UNPOWRED.";
                if (_drillCon.Deployed) return "DRILL ACTIVE.";
                else return "STOWING DRILL.";
            };
            var info = UIPrefabHelpers.Current.GetInfoBox(genText);
            info.transform.SetParent(inspectorPane);
        }
    }
}

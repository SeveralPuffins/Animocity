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
    public class BuildingComponent_GiantGlove: BuildingComponent
    {
        private BoxingGlovePhysicsController _bgController;
        public BuildingComponent_GiantGlove(BuildingComponentData data, Building building) : base(data, building)
        {
            this._bgController = building.GetComponentInChildren<BoxingGlovePhysicsController>();
            UpdateGloveParams();
        }

        public BuildingComponentData_GiantGlove GloveData
        {
            get
            {
                return Data as BuildingComponentData_GiantGlove;
            }
        }

        
        protected override bool Tick(Building building)
        {
            UpdateGloveParams();

            return base.Tick(building);
        }

        private void UpdateGloveParams()
        {
            _bgController.recoverySpeed = GloveData.BaseWindingSpeed*this.Building.BuildingEfficiency;
            _bgController.launchImpulse = GloveData.launchVelocity;
            _bgController.maxDistance = GloveData.maxDistance;
        }

        protected override bool HasInspector() => true;

        protected override void PopulateInspectorContentPane(Transform inspectorPane)
        {
            Func<string> genText = () =>
            {
                if (_bgController.Ready) return "LOADED.";
                else return "RELOADING.";
            };
            var info = UIPrefabHelpers.Current.GetInfoBox(genText);
            info.transform.SetParent(inspectorPane);
        }
    }
}

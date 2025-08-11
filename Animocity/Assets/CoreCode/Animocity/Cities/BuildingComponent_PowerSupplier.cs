using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingComponent_PowerSupplier : BuildingComponent_Power
    {
        public BuildingComponent_PowerSupplier(BuildingComponentData data, Building building) : base(data, building){}


        protected override void PopulateInspectorContentPane(Transform inspectorPane)
        {
            var txt = inspectorPane.GetComponentInChildren<TMP_Text>();
            txt.text = $"On: Produicing {this.PowerData.powerAmount}MW";
            base.PopulateInspectorContentPane(inspectorPane);

        }
    }
}
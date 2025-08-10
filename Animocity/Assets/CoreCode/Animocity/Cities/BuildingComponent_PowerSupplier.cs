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


        protected override bool Tick(Building building)
        {
            return base.Tick(building);
        }

        protected override void populateInspectorPane(Transform contentPane)
        {
            var txt = contentPane.GetComponentInChildren<TMP_Text>();
            txt.text = $"On: Produicing {this.PowerData.powerAmount}MW";
            base.populateInspectorPane(contentPane);

        }
    }
}
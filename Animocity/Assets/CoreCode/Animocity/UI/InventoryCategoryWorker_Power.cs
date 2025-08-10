using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Animocity.Cities;
using BlueprintSystem;
using UnityEngine.UI;

namespace Animocity.UI
{
    public class InventoryCategoryWorker_Power : InventoryCategoryWorker
    {
        public override string GetDisplayText()
        {
            float stored = 0f;
            float prod = CityOverview.Current.PowerSupply - CityOverview.Current.PowerDemand;

            return String.Format("{0:0.0} :: {1:0.0}", stored, prod);
        }

        public override void OnInventoryButtonClick(Button btn)
        {
            
        }
    }
}

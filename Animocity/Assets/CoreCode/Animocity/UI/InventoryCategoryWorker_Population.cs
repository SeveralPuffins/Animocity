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
    public class InventoryCategoryWorker_Population : InventoryCategoryWorker
    {
        private Transform panel = null;
        public override string GetDisplayText()
        {
            float pop = CityOverview.Current.TotalPopulation;
            return $" {pop} : (-)";
        }

        public override void OnInventoryButtonClick(Button btn)
        {
            if (panel!=null)
            {
                Close();    
            }
            else
            {
                panel = MakeDropdown(btn.transform); 

                foreach (var blue in BlueprintDatabase<PopulationBlue>.FetchAll())
                {
                    var entry = GetInventoryEntryForBlue(blue);
                    entry.textUpdateFunc = () => { return $"{blue.DisplayName} :: {CityOverview.Current.GetPopulationByClass(blue)}"; };
                    entry.transform.SetParent(panel);
                }
            }
        }

        public void Close()
        {
            GameObject.Destroy(panel.gameObject);
        }

        private Transform MakeDropdown(Transform parent)
        {
            var dropdown = UIPrefabHelpers.Current.GetInventoryDropdown();

            dropdown.SetParent(parent.transform);
            dropdown.localPosition = new Vector3(0,-30,0);

            return dropdown;
        }

        private InventoryListerOption GetInventoryEntryForBlue(PopulationBlue blue)
        {
            var entryPanel = UIPrefabHelpers.Current.GetInventoryEntry();
            InventoryListerOption entry = entryPanel.GetComponent<InventoryListerOption>();
            entry.SetSprite(blue.GetSprite());

            return entry;
        }
    }
}

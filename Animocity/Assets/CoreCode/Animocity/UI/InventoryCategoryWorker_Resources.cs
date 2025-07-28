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
    public class InventoryCategoryWorker_Resources : InventoryCategoryWorker
    {
        private Transform panel = null;
        public override string GetDisplayText()
        {
            float stored = CityInventory.Current.GetTotalWhere((b) => true);
            float edible = CityInventory.Current.GetTotalWhere((b) => b.edible);

            return String.Format("{0:0.0} :: {1:0.0}", stored,edible);
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

                foreach (var blue in BlueprintDatabase<ResourceBlue>.FetchAll())
                {
                    var entry = GetInventoryEntryForBlue(blue);
                    entry.textUpdateFunc = () => { return String.Format("{0} :: {1:0.0}", blue.DisplayName, CityInventory.Current.GetResourceAmount(blue)); };
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

        private InventoryListerOption GetInventoryEntryForBlue(ResourceBlue blue)
        {
            var entryPanel = UIPrefabHelpers.Current.GetInventoryEntry();
            InventoryListerOption entry = entryPanel.GetComponent<InventoryListerOption>();
            entry.SetSprite(blue.GetSprite());

            return entry;
        }
    }
}

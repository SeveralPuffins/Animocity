using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Animocity.Cities;
using UnityEngine.UI;

namespace Animocity.UI
{
    public class UIPrefabHelpers : MonoBehaviour
    {
        public Transform InventoryEntryPrefab;
        public Transform InventoryDropdownPrefab;
        public ProductionTimerPanel ProductionTimerPanel;
        public BuildingInspectorComp Inspector;
        public Button InspectorTabButton;
        public static UIPrefabHelpers Current { get; private set; }

        public void Awake()
        {
            Current = this;
        }

        public Transform GetInventoryEntry()
        {
            return Transform.Instantiate(InventoryEntryPrefab);
        }
        public ProductionTimerPanel GetProductionTimerPanel(BuildingComponent_Production prod)
        { 
            var panel = Instantiate<ProductionTimerPanel>(ProductionTimerPanel);
            panel.SetSource(prod);
            return panel;
        }
        public Transform GetInventoryDropdown()
        {
            return Transform.Instantiate(InventoryDropdownPrefab);
        }
        public BuildingInspectorComp PopulateInspector(Building building)
        {
            Inspector.gameObject.SetActive(true);
            Inspector.SetBuilding(building);
            return Inspector;
        }
        public void DectivateInspector(Building building)
        {
            Inspector.Clear();
        }

        internal Button GetInspectorButton()
        {
            return Instantiate<Button>(InspectorTabButton);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.UI
{
    public class UIPrefabHelpers : MonoBehaviour
    {
        public Transform InventoryEntryPrefab;
        public Transform InventoryDropdownPrefab;
        public static UIPrefabHelpers Current { get; private set; }

        public void Awake()
        {
            Current = this;
        }

        public Transform GetInventoryEntry()
        {
            return Transform.Instantiate(InventoryEntryPrefab);
        }
        public Transform GetInventoryDropdown()
        {
            return Transform.Instantiate(InventoryDropdownPrefab);
        }
    }
}

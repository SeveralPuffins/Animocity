using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Animocity.UI
{
    public class InventoryCategoryWorker
    {
        public virtual string GetDisplayText()
        {
            return "BASE INVENTORY TEXT";
        }

        public virtual void OnInventoryButtonClick(Button btn)
        {
            
        }
    }
}

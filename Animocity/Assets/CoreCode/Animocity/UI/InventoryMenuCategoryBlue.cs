using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Animocity.Cities;
using BlueprintSystem;
using UnityEngine;

namespace Animocity.UI
{
    public class InventoryMenuCategoryBlue : Blueprint
    {
        public string iconPath;
        
        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(iconPath);
        }


        private InventoryCategoryWorker worker;
        public InventoryCategoryWorker Worker
        {
            get
            {
                if (worker == null) worker = (InventoryCategoryWorker)Activator.CreateInstance(this.inventoryCategoryWorker);
                return worker;
            }
        }
        public Type inventoryCategoryWorker;
    }
}

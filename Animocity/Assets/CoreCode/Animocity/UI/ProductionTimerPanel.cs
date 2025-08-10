using Animocity.Cities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Animocity.UI
{
    public class ProductionTimerPanel : MonoBehaviour
    {
        private BuildingComponent_Production productionComp;
        public TMP_Text productionText;
        public Image img;
        public void SetSource(BuildingComponent_Production productionComp)
        {
            this.productionComp = productionComp;
            
        }

        private void Update()
        {
            var targetImg = productionComp.SelectedProcess.outputs.First().Key.GetSprite();
            if(this.img.sprite!=targetImg)
            {
                this.img.sprite = targetImg;
            }

            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if(this.productionComp.SelectedProcess != null)
            {
                float progress = productionComp.CurrentProgress / productionComp.SelectedProcess.productivityCost;
                productionText.text = $"{(int)(100.0*progress)}%";

                img.fillAmount = progress;
            }
        }
    }
}

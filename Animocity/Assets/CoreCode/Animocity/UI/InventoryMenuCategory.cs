using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Animocity.UI
{
    public class InventoryMenuCategory : MonoBehaviour
    {

        public Image icon;
        public TMP_Text text;
        private bool _init = false;

        public InventoryMenuCategoryBlue Blue { get; private set; }

        public void Initialise(InventoryMenuCategoryBlue blue)
        {
            this.Blue = blue;
            icon.sprite = blue.GetSprite();
            ConnectButton(blue);            

            _init = true;
        }

        private void ConnectButton(InventoryMenuCategoryBlue blue)
        {
            var btn = this.GetComponent<Button>();
            btn.onClick.AddListener(() => blue.Worker.OnInventoryButtonClick(btn));
        }

        // Update is called once per frame
        void Update()
        {
            if(!_init) return;

            text.text = Blue.Worker.GetDisplayText();
        }
    }
}
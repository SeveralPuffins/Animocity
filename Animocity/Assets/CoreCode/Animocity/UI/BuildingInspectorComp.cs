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
    public class BuildingInspectorComp : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text description;
        public Button close;
        public Transform tabPane;
        public Transform contentPane;

        private void Awake()
        {
            close.onClick.AddListener(()=> this.Clear());
        }

        private int layerOfOrigin;
        private Building building;
        internal void SetBuilding(Building building)
        {
            this.Clear(false);
            this.building = building;
            SelectHighlight();
            this.titleText.text = building.Blue.DisplayName;
            this.description.text = building.Blue.description;

            var comps = building.GetComps<BuildingComponent>();

            foreach (var comp in building.GetComps<BuildingComponent>())
            {
                comp.AddInspectorInfo(this);
            }
        }

        private void Unselect()
        {
            if(building != null)
            {
                building.gameObject.layer = layerOfOrigin;
                foreach (Transform t in building.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = layerOfOrigin;
                }
            }
        }
        private void SelectHighlight()
        {
            if (this.building != null)
            {
                layerOfOrigin = this.building.gameObject.layer;
                building.gameObject.layer = 8;
                foreach (Transform t in building.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = 8;
                }
            }
        }

        internal void Clear(bool close = true)
        {
            titleText.text = "";
            description.text = "";
            ClearContentPane();
            ClearTabPane();
            if (close)
            {
                this.gameObject.SetActive(false);
            }
            Unselect();
        }

        internal void ClearContentPane()
        {
            foreach (Transform item in contentPane)
            {
                if (item.tag != "NoClear")
                {
                    Destroy(item.gameObject);
                }
            }
            contentPane.GetComponentInChildren<TMP_Text>().text = "";
        }
        internal void ClearTabPane()
        {
            foreach (Transform item in tabPane)
            {
                Destroy(item.gameObject);
            }
        }
    }
}

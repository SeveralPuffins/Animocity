using Animocity.Cities;
using BlueprintSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Animocity.UI
{
    // It would be nice to have a more general version of this for any blueprint type. 
    public class BuildingBlueprintSelector : MonoBehaviour
    {
        public static BuildingBlueprint Selected { get; private set; }
        public Transform UIButtonPrefab;
        void Start()
        {
            DataLoader.OnDataLoaded += this.OnDataUpdated;
        }

        private void OnDataUpdated(PlayerProfile pro, DataLoader.LoadStatus status)
        {
            if(status == DataLoader.LoadStatus.LOADED)
            {
                Clear();
                //LoadAllBuildings();
            }
        }

        private void OnDestroy()
        {
            DataLoader.OnDataLoaded -= this.OnDataUpdated;
        }

        private void Clear()
        {
            foreach (Transform item in transform)
            {
                GameObject.Destroy(item.gameObject);
            }
        }

        private void LoadAllBuildings()
        {
            foreach (var buildingBlue in BlueprintDatabase<BuildingBlueprint>.FetchAll()) {
                var newButtonTransform  = Instantiate<Transform>(UIButtonPrefab, this.transform);
                var newButton           = newButtonTransform.GetComponent<Button>();
                var txt                 = newButtonTransform.GetComponentInChildren<TMP_Text>();

                txt.text = buildingBlue.DisplayName;


                newButton.onClick.AddListener(() => {
                    Selected = buildingBlue;
                    new ControlContext_Builder(Selected).Activate();
                });
            }
        }

        internal void DisplayCategory(BuildingCategoryBlueprint cat)
        {
            Clear();
            foreach (var buildingBlue in BlueprintDatabase<BuildingBlueprint>.FetchAllWhere((blue)=>blue.category==cat))
            {
                var newButtonTransform = Instantiate<Transform>(UIButtonPrefab, this.transform);
                var newButton = newButtonTransform.GetComponent<Button>();
                var txt = newButtonTransform.GetComponentInChildren<TMP_Text>();

                txt.text = buildingBlue.DisplayName;


                newButton.onClick.AddListener(() => {
                    Selected = buildingBlue;
                    new ControlContext_Builder(Selected).Activate();
                });
            }

        }
    }
}
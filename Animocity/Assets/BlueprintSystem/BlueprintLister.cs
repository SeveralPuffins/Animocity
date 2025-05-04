using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueprintSystem;

namespace BlueprintSystem
{
    [RequireComponent(typeof(Mask))]
    public class BlueprintLister<T> where T : Blueprint, new()
    {

        public bool multiSelect = false;
        private Transform UIEntry;
        private Transform content;

        private List<T> selectedBlueprints = new List<T>();
        public List<T> SelectedBlueprints
        {
            get
            {
                return selectedBlueprints;
            }
        }

        private T lastSelected;
        public T LastSelected
        {
            get
            {
                return lastSelected;
            }
        }

        public BlueprintLister(Transform entryPrefab, Transform contentPane, bool multiSelect)
        {
            this.content = contentPane;
            this.UIEntry = entryPrefab;
            this.multiSelect = multiSelect;
            MakeList();
        }

        private void MakeList()
        {
            IEnumerable<T> blues = BlueprintDatabase<T>.FetchAll();
            foreach (T blue in blues)
            {
                Transform entry = MakeUIEntryForPart(blue);
                entry.SetParent(content);
            }
        }

        private Transform MakeUIEntryForPart(T blue)
        {
            Transform entry = Transform.Instantiate<Transform>(UIEntry);
            entry.Find("PartName").GetComponent<Text>().text = blue.DisplayName;
            Toggle toggle = entry.Find("Toggle").GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(
                delegate {
                    if (toggle.isOn)
                    {
                        if (!multiSelect)
                        {
                            selectedBlueprints.Clear();
                            foreach (Toggle t in content.GetComponentsInChildren<Toggle>())
                            {
                                if (t != toggle) t.isOn = false;
                            }
                        }
                        selectedBlueprints.Add(blue);
                        lastSelected = blue;
                    }
                    else
                    {
                        selectedBlueprints.Remove(blue);
                    }
                }
            );
            return entry;
        }
    }
}


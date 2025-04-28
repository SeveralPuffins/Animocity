using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.UIElements;
using BlueprintSystem;

namespace BlueprintSystem
{
    [RequireComponent(typeof(Mask))]
    public class BlueprintPicker<T> where T : Blueprint, new()
    {
        private Transform content;
        private Dropdown dropdown;
        private List<T> blueprints;
        public Dropdown Dropdown
        {
            get
            {
                return dropdown;
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

        public BlueprintPicker(Transform contentPane, Dropdown dropdown)
        {
            this.content = contentPane;
            this.dropdown = dropdown;
            dropdown.onValueChanged.AddListener((e) => this.UpdateChoice());
            MakeList();
            lastSelected = blueprints[dropdown.value];
        }

        private void MakeList()
        {
            this.blueprints = BlueprintDatabase<T>.FetchAll().ToList();
            List<Dropdown.OptionData> opts = new List<Dropdown.OptionData>();
            foreach (T blue in blueprints)
            {
                Dropdown.OptionData opt = new Dropdown.OptionData();
                opt.text = blue.DisplayName;
                opts.Add(opt);

            }
            dropdown.ClearOptions();
            dropdown.AddOptions(opts);

        }

        private void UpdateChoice()
        {
            lastSelected = blueprints[dropdown.value];
        }
    }
}


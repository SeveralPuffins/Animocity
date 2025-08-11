using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Animocity.UI
{
    public class PrioritySwitcher : MonoBehaviour
    {
        public Color handleOff;
        public Color handleOn;
        public Color handlePriority;

        public Slider prioritySlider;
        public Image sliderHandle;

        public delegate void PriorityChange(int priority);
        public event PriorityChange PriorityChanged;

        void Awake()
        {
            prioritySlider.onValueChanged.AddListener((f) => this.OnPriorityChanged(f));
            OnPriorityChanged(prioritySlider.value);
        }

        public void ForceSetPriority(int priority)
        {
            prioritySlider.value = priority;
        }

        private void OnPriorityChanged(float f)
        {
            if (f == 0)
            {
                sliderHandle.color = handleOff;
            }
            else if(f == 1)
            {
                sliderHandle.color = handleOn;
            }
            else if (f == 2)
            {
                sliderHandle.color = handlePriority;
            }

            PriorityChanged?.Invoke((int)f);
        }
    }
}

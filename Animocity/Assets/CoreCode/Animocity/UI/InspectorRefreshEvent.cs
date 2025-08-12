using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Animocity.UI
{
    public class InspectorRefreshEvent : EventBase<InspectorRefreshEvent>
    {
        public MonoBehaviour source { get; private set; }

        protected override void Init()
            {
                base.Init();
                bubbles = true;
                tricklesDown = false;
        }
        
    
    }
}

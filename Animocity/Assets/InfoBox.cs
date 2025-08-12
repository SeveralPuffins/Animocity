using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Animocity.UI
{ 
    public class InfoBox : MonoBehaviour
    {
        public TMP_Text txt;
        public Func<string> getInfoString;

        void Update()
        {
            txt.text = getInfoString();
        }
    }
}
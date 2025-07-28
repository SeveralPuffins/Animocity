using Animocity.UI;
using BlueprintSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class PopulationBlue : Blueprint
    {
        public string description;
        public string iconPath;
        public int startingPopulation;
        public bool birthType;

        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(iconPath);
        }
    }
}

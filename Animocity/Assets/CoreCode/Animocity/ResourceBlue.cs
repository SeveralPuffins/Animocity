using Animocity.Cities;
using BlueprintSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity
{
    public class ResourceBlue : Blueprint
    {
        public float value;
        public bool edible;
        public string description;
        public string iconPath;

        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(iconPath);
        }
    }
}

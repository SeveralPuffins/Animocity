using BlueprintSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Animocity.UI 
{
    public class BuildingCategoryBlueprint : Blueprint
    {
        public string iconPath;
        public string description;
        public Color categoryColour;

        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(iconPath);
        }
    }
}
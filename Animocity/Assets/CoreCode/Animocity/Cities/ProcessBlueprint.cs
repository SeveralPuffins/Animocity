using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Animocity.UI;
using BlueprintSystem;
using UnityEngine;

namespace Animocity.Cities
{
    public class ProcessBlueprint : Blueprint
    {
        public override string DisplayName => displayName;

        public string displayName;
        public string description;

        public float productivityCost;

        public Dictionary<ResourceBlue, float> inputs;
        public Dictionary<ResourceBlue, float> outputs;

        
    }
}

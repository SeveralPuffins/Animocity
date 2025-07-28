using Animocity.Cities;
using BlueprintSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity
{
    public class ScenarioBlue : Blueprint
    {
        public bool isDefault;
        public string description;
        public Dictionary<PopulationBlue, int> startingPopulations;
        public Dictionary<ResourceBlue, float> startingResources;
    }
}

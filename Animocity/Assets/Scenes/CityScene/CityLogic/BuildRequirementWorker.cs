using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public abstract class BuildRequirementWorker
    {
        public virtual bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, BuildingGrid buildingGrid)
        {
            return false;
        }
    }
}

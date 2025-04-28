using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildRequirementWorker_Fits : BuildRequirementWorker
    {
        public override bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, BuildingGrid buildingGrid)
        {
            if(buildingBlue.tilesNeeded==null) return false;

            foreach(var tile in buildingBlue.tilesNeeded)
            {
                if (buildingGrid.IsOccupied(tile + location))
                {
                    return false;
                }
            }
            return true;

        }
    }
}

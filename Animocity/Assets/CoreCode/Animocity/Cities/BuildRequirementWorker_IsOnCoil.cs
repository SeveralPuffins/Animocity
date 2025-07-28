using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Animocity.Cities
{
    public class BuildRequirementWorker_IsOnCoil : BuildRequirementWorker
    {
       

        public override bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, CityGrid buildingGrid)
        {
            foreach (var tile in buildingBlue.tilesNeeded)
            {
                if (!buildingGrid.IsInBounds(tile + location))
                {
                    return false;
                }
                if (buildingGrid.TryGetBuildingAt(location, out var maybeACoil))
                {
                    return maybeACoil.Blue.label == "PowerCoil";
                }
            }

            

            return false;
        }
    }
}

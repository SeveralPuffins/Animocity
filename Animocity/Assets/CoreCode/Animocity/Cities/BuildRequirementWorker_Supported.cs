using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Animocity.Cities
{
    public class BuildRequirementWorker_Supported : BuildRequirementWorker
    {
        public BuildRequirementWorker_Supported(BuildRequirementBlueprint blue) : base(blue) { }
        private List<Vector2Int> GetAllTilesNeedingSupport(BuildingBlueprint buildingBlue)
        {
            return buildingBlue.tilesNeeded.Where(el => el.y <= 0).ToList();
        }

        public override bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, CityGrid buildingGrid)
        {
            return GetAllTilesNeedingSupport(buildingBlue).All((offset) => buildingGrid.IsSupported(offset+location));
        }

        public override void OnBuildAtLocation(Vector2Int location, Building building, CityGrid buildingGrid)
        {
            var supportingBuildings = new HashSet<Building>();
            foreach(var tile in GetAllTilesNeedingSupport(building.Blue))
            {
                if(buildingGrid.TryGetBuildingAt(tile+location+Vector2Int.down, out var supporter))
                {
                    supportingBuildings.Add(supporter);
                }
            }

            building.SubscribeToSupporters(supportingBuildings);
        }
    }
}

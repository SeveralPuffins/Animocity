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
    }
}

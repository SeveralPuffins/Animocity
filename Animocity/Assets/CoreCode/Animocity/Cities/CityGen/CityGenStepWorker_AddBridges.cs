using Animocity.Cities;
using BlueprintSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animocity.Cities.CityGen
{
    public class CityGenStepWorker_AddBridges : CityGenStepWorker
    {
        private BuildingBlueprint gantry;

        public CityGenStepWorker_AddBridges(CityGeneratorStepBlue blue) : base(blue)
        {

        }

        public override void Run(List<CityGrid> cityGrids)
        {
            this.gantry = BlueprintDatabase<BuildingBlueprint>.Fetch("Gantry");

            foreach (CityGrid cityGrid in cityGrids)
            {
                foreach (var bridgePoint in cityGrid.externalConnectionPoints)
                {
                    if (!cityGrid.TryGetBuildingAt(bridgePoint+Vector2Int.down, out var building))
                    {
                        cityGrid.TryBuildAtLocation(gantry, bridgePoint + Vector2Int.down, out var newBuilding, isFree: true);
                    }
                }
            }
        }
    }
}
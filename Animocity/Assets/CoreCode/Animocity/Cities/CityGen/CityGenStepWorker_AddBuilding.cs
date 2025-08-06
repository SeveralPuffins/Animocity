using Animocity.Cities;
using BlueprintSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Animocity.Cities.CityGen
{
    public class CityGenStepWorker_AddBuilding : CityGenStepWorker
    {
        public CityGenStepWorker_AddBuilding(CityGeneratorStepBlue blue) : base(blue)
        {

        }

        public override void Run(List<CityGrid> cityGrids)
        {
            foreach (CityGrid cityGrid in cityGrids)
            {
                if (Blue.stringB.Equals(string.Empty) || cityGrid.gridTags.Contains(Blue.stringB))
                {
                    if (Blue.stringC.Equals(string.Empty) || (!cityGrid.gridTags.Contains(Blue.stringC)))
                    {
                        AddBuildings(cityGrid);
                    }
                }
            }
        }

        private void AddBuildings(CityGrid cityGrid)
        {
            BuildingBlueprint buildingBlue = BlueprintDatabase<BuildingBlueprint>.Fetch(Blue.stringA);

            for (int n=0; n<Blue.paramA; n++)
            {
                AddBuilding(buildingBlue, cityGrid);
            }

        }

        private void AddBuilding(BuildingBlueprint buildingBlue, CityGrid cityGrid)
        {
            var bounds = cityGrid.TileBounds;

            for (int retries = 0; retries < 10; retries++)
            {
                var seed = new Vector2Int
                                (
                                    Random.Range(bounds.xMin, bounds.xMax),
                                    Random.Range(bounds.yMin, bounds.yMax)
                                );

                if(TryBuildAtFirstViableLocationUnderPoint(cityGrid, seed, buildingBlue)){
                    return;
                }
            }

        }

        private bool TryBuildAtFirstViableLocationUnderPoint(CityGrid cityGrid, Vector2Int seed, BuildingBlueprint buildingBlue)
        {
            var bounds = cityGrid.TileBounds;
            for (int j = seed.y; j > bounds.yMin; j--)
            {
                var loc = new Vector2Int(seed.x, j);
                if (buildingBlue.CanBuildAtLocation(loc, cityGrid))
                {
                    cityGrid.TryBuildAtLocation(buildingBlue, loc, out var newBuilding, isFree: true);
                    return true;
                }
            }
            return false;
        }
    }
}
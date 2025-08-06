using Animocity.Cities;
using BlueprintSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animocity.Cities.CityGen
{
    public class CityGenStepWorker_AddGantries : CityGenStepWorker
    {
        private BuildingBlueprint gantry;

        public CityGenStepWorker_AddGantries(CityGeneratorStepBlue blue) : base(blue)
        {

        }

        public override void Run(List<CityGrid> cityGrids)
        {
            this.gantry = BlueprintDatabase<BuildingBlueprint>.Fetch("Gantry");

            foreach (CityGrid cityGrid in cityGrids)
            {
                if (!cityGrid.gridTags.Contains("Surface"))
                {
                    AddBaselineGantry(cityGrid);
                }
                if (cityGrid.gridTags.Contains("MainBody"))
                {
                    AddAdditionalGantries(cityGrid);
                }
            }
        }

        private void AddAdditionalGantries(CityGrid cityGrid)
        {
            

            for(int n=0; n<Blue.paramA; n++)
            {
                AddPerpendicularGantry(cityGrid, (int)Blue.paramB);
            }

        }

       

        private void AddPerpendicularGantry(CityGrid cityGrid, int clearance, int retriesLeft = 5)
        {
            var bounds = cityGrid.TileBounds;

            var seed = new Vector2Int
                            (
                                Random.Range(bounds.xMin + clearance, bounds.xMax - clearance),
                                Random.Range(bounds.yMin + clearance, bounds.yMax - clearance)
                            );

           if(HasClearance(cityGrid, seed, clearance))
           {
                if(Random.value < 0.5f)
                {
                    AddVerticalGantry(bounds, cityGrid, seed);
                }
                else
                {
                    AddHorizontalGantry(bounds, cityGrid, seed);
                }
           }
           // else if(retriesLeft > 0)
           // {
                //AddPerpendicularGantry(cityGrid, clearance, retriesLeft-1);
           // }

        }

        private void AddHorizontalGantry(RectInt bounds, CityGrid cityGrid, Vector2Int seed)
        {
            for (int i = seed.x; i >= bounds.xMin; i--)
            {
                var newPt = new Vector2Int(i, seed.y);
                if (cityGrid.TryGetBuildingAt(newPt, out var building))
                {
                    break;
                }
                else
                {
                    cityGrid.TryBuildAtLocation(gantry, newPt, out var newBuilding, isFree:true);
                }
            }

            for (int i = seed.x + 1; i <= bounds.xMax; i++)
            {
                var newPt = new Vector2Int(i, seed.y);
                if (cityGrid.TryGetBuildingAt(newPt, out var building))
                {
                    break;
                }
                else
                {
                    cityGrid.TryBuildAtLocation(gantry, newPt, out var newBuilding, isFree: true);
                }
            }
        }

        private void AddVerticalGantry(RectInt bounds, CityGrid cityGrid, Vector2Int seed)
        {
            for(int j = seed.y; j>=bounds.yMin; j--)
            {
                var newPt = new Vector2Int(seed.x, j);
                if (cityGrid.TryGetBuildingAt(newPt, out var building))
                {
                    break;
                }
                else
                {

                    cityGrid.TryBuildAtLocation(gantry, newPt, out var newBuilding, isFree: true);
                }
            }

            for (int j = seed.y+1; j <= bounds.yMax; j++)
            {
                var newPt = new Vector2Int(seed.x, j);
                if (cityGrid.TryGetBuildingAt(newPt, out var building))
                {
                    break;
                }
                else
                {
                    cityGrid.TryBuildAtLocation(gantry, newPt, out var newBuilding, isFree: true);
                }
            }
        }

        private bool HasClearance(CityGrid cityGrid, Vector2Int point, int clearance)
        {
            for(int i=point.x-clearance; i<=point.x+clearance; i++) 
            {
                for (int j = point.y - clearance; j <= point.y + clearance; j++)
                {
                    if(cityGrid.IsOccupied(new Vector2Int(i,j)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void AddBaselineGantry(CityGrid cityGrid)
        {
            cityGrid.GetBaseTiles().ForEach(tile =>
            {
                if(cityGrid.TryBuildAtLocation(gantry, tile, out var newGantry, isFree: true))
                {

                }
                else
                {
                    MonoBehaviour.print($"Failed to build {gantry.DisplayName} at {tile}.");
                }
            });
        }
    }
}
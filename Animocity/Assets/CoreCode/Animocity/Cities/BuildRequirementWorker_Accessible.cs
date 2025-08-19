using Animocity.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Animocity.Cities
{
    public class BuildRequirementWorker_Accessible : BuildRequirementWorker
    {
        public BuildRequirementWorker_Accessible(BuildRequirementBlueprint blue) : base(blue) { }
        private List<Vector2Int> GetTilesAdjacentToRoot(Vector2Int location)
        {
            return new List<Vector2Int>
            {
                new Vector2Int(0, 1) + location,
                new Vector2Int(0, -1) + location,
                new Vector2Int(-1, 0) + location,
                new Vector2Int(-1, -1) + location
            };
        }

        public override bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, CityGrid buildingGrid)
        {
            var tiles = GetTilesAdjacentToRoot(location);

            return tiles.WhereAny(tile =>
            {
                if (buildingGrid.TryGetBuildingAt(tile, out var adjacentBuilding))
                {
                    var transportSquares = adjacentBuilding.GetComps<BuildingComponent_Transport>();
                    if(transportSquares!=null && transportSquares.Count() > 0)
                    {
                        var tiles = TransportManager.Current.GetConnectedTiles(buildingGrid, location, TransportManager.MAX_COMMUTE_COST);

                        foreach (var t in tiles) 
                        {
                            buildingGrid.Highlight(t, buildingGrid.highlightInformational);
                        }
                        return true;
                    }
                }
                return false;
            });
        }
    }
}

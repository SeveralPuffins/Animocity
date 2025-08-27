using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Animocity.Cities
{
    public class BuildRequirementWorker_HighlightRange : BuildRequirementWorker
    {
        
        public BuildRequirementWorker_HighlightRange(BuildRequirementBlueprint blue) : base(blue) { }

        public BuildRequirementBlueprint_HighlightRange RangeBlue
        {
            get
            {
                return (BuildRequirementBlueprint_HighlightRange)this.Blue;
            }
        }

        public override bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, CityGrid buildingGrid)
        {
            var mp = new MultiPoint(location, CityOverview.Current.CityMultiGrid.GetIndex(buildingGrid));
            var tiles = TransportManager.Current.GetConnectedTiles(mp, RangeBlue.highlightRange);

                foreach (var t in tiles)
                {
                    CityOverview.Current.CityMultiGrid.Highlight(t, buildingGrid.highlightInformational2);
                }
                return true;
        }
    }
}

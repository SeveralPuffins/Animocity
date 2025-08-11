using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Animocity.Cities
{
    public class BuildRequirementWorker_GridHasTag : BuildRequirementWorker
    {
        BuildRequirementBlueprint_GridTag TagBlue { get; set; }
        public BuildRequirementWorker_GridHasTag(BuildRequirementBlueprint blue) : base(blue)
        {
            MonoBehaviour.print("XXXXXXXXXXXXXXXX SUCCESSFULLY MADE WORKER! XXXXXXXXXXXXXXXXXXXXX");
            this.TagBlue = (BuildRequirementBlueprint_GridTag)blue;
            MonoBehaviour.print($"XXXXXXXXXXXXXXXX TAG IS {TagBlue.tag}! XXXXXXXXXXXXXXXXXXXXX");
        }

        public override bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, CityGrid buildingGrid)
        {
            return buildingGrid.gridTags.Contains(TagBlue.tag);
        }

        public override string GetErrorMessage()
        {
            return string.Format(Blue.failMessage, TagBlue.tag);
        }
    }
}

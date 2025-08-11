using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public abstract class BuildRequirementWorker
    {
        protected BuildRequirementBlueprint Blue { get; set; }
        protected BuildRequirementWorker(BuildRequirementBlueprint blue)
        {
            MonoBehaviour.print($"MADE BRWorker {this.GetType().Name} with blue {blue.DisplayName}");
            this.Blue = blue;
        }
        public virtual bool CanBuildAtLocation(Vector2Int location, BuildingBlueprint buildingBlue, CityGrid buildingGrid)
        {
            return false;
        }

        public virtual string GetErrorMessage()
        {
            return Blue.failMessage;
        }
    }
}

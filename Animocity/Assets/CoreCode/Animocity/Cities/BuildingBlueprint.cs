using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Animocity.UI;
using BlueprintSystem;
using UnityEngine;

namespace Animocity.Cities
{
    public class BuildingBlueprint : Blueprint
    {
        public override string DisplayName => displayName;

        public string displayName;
        public string description;
        public BuildingCategoryBlueprint category;
        public string prefabPath;


        public float powerUse;
        public int workersNeeded;

        public bool needsSupport;
        public bool grantsSupport;
        public bool autoRow;


        public List<Vector2Int> tilesNeeded;

        public List<BuildRequirementBlueprint> buildRequirements;
        public List<BuildingComponentData> components;

        public bool CanBuildAtLocation(Vector2Int loc, CityGrid grid)
        {
            if(buildRequirements == null || buildRequirements.Count  == 0)
            {
                return true;
            }
           
            foreach(var req in buildRequirements)
            {
                if(!req.Worker.CanBuildAtLocation(loc, this, grid))
                {
                    return false;
                }
            }
            return true;
        }

        private Transform _cachedPrefab;
        public Transform GetPrefab()
        {
            if (!_cachedPrefab)
            {
                MonoBehaviour.print($"Loading resource from path in mod Resource folder: {prefabPath}");
                _cachedPrefab =  Resources.Load<Transform>(prefabPath);
            }
            return _cachedPrefab;
        }

        public int Width
        {
            get
            {
                return 1 + (tilesNeeded.Max(v2i => v2i.x) - tilesNeeded.Min(v2i => v2i.x));
            }
        }
        public int Height
        {
            get
            {
                return 1 + (tilesNeeded.Max(v2i => v2i.y) - tilesNeeded.Min(v2i => v2i.y));
            }
        }
    }
}

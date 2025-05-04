using Animocity.Cities;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Animocity.UI
{
    public class ControlContext_Builder : ControlContext
    {
        private const int HIGHLIGHT_DIST = 5;
        private BuildingBlueprint _selected;
        public ControlContext_Builder(BuildingBlueprint selectedBuilding)
        { 
            this._selected = selectedBuilding;
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Release()
        {
            base.Release();
        }


        public override void OnHover(BuildingGrid grid, Vector3 positionWorld)
        {
            HighlightBuildingOutline(grid, grid.WorldToCell(positionWorld));
        }

        public override void OnInteract(BuildingGrid grid, Vector3 positionWorld)
        {

            if (grid.TryBuildAtLocation(_selected, grid.WorldToCell(positionWorld), out Building newBuilding))
            {

            }
            else
            {
                // Screen shake? Error noise?
            }
        }

        public override void OnInspect(BuildingGrid grid, Vector3 positionWorld)
        {
            Release();
        }

   
        private void HighlightBuildingOutline(BuildingGrid grid, Vector2Int pt)
        {
            float alpha = 0.5f;

            Color hClr = _selected.CanBuildAtLocation(pt, grid) 
                            ? grid.highlightPositive.WithAlpha(alpha)
                            : grid.highlightNegative.WithAlpha(alpha);

            foreach (var tileOffset in _selected.tilesNeeded)
            {
                        grid.Highlight(pt+tileOffset, hClr);
            }
        }
    }
}

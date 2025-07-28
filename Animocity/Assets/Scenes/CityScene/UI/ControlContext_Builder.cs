using Animocity.Cities;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
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


        public override void OnHover(CityGrid grid, Vector3 positionWorld, bool drag, Vector3 dragFrom)
        {
            if (!drag)
            {
                HighlightBuildingOutline(grid, grid.WorldToCell(positionWorld));
            }
            else
            {
                var stride2D = new Vector2Int(_selected.Width, _selected.Height);

                var dragSquares = grid.GetSquaresBetween(dragFrom, positionWorld, stride2D, lockLongestRowCol: true);

                foreach (var square in dragSquares)
                {
                    HighlightBuildingOutline(grid, square);
                }
            }
        }

      

        public override void OnInteract(CityGrid grid, Vector3 positionWorld, bool drag, Vector3 dragFrom)
        {
            if(drag && _selected.autoRow)
            {
                var stride = new Vector2Int(_selected.Width, _selected.Height);
                var dragSquares = grid.GetSquaresBetween(dragFrom, positionWorld, stride, lockLongestRowCol:true);

                foreach(var square in dragSquares)
                {
                    TryBuildAtLocation(grid, square);
                }
            }
            else 
            {
                TryBuildAtLocation(grid, grid.WorldToCell(positionWorld));
            }
        }

        private void TryBuildAtLocation(CityGrid grid, Vector2Int position)
        {
            if (grid.TryBuildAtLocation(_selected, position, out Building newBuilding))
            {

            }
            else
            {
                // Screen shake? Error noise?
            }
        }

        public override void OnInspect(CityGrid grid, Vector3 positionWorld)
        {
            Release();
        }

   
        private void HighlightBuildingOutline(CityGrid grid, Vector2Int pt)
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

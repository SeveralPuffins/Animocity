using Animocity.Cities;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Animocity.UI
{
    public class ControlContext_Inspector : ControlContext
    {
        private const int HIGHLIGHT_DIST = 5;
        public ControlContext_Inspector()
        { 
            SetDefault(this);
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
            HighlightLocalGrid(grid, grid.WorldToCell(positionWorld));
        }

        public override void OnInteract(BuildingGrid grid, Vector3 positionWorld)
        {

        }

        public override void OnInspect(BuildingGrid grid, Vector3 positionWorld)
        {

        }

   
        private void HighlightLocalGrid(BuildingGrid grid, Vector2Int ctr)
        {
           
            var minCell = ctr - HIGHLIGHT_DIST*Vector2Int.one;
            float alpha;

            for (int i = minCell.x; i <= minCell.x + 2 * HIGHLIGHT_DIST; i++)
            {
                for (int j = minCell.y; j <= minCell.y + 2 * HIGHLIGHT_DIST; j++)
                {
                    float dX = (minCell.x + HIGHLIGHT_DIST - i) * 1f / (0.5f + HIGHLIGHT_DIST);
                    float dY = (minCell.y + HIGHLIGHT_DIST - j) * 1f / (0.5f + HIGHLIGHT_DIST);
                    float dst = Mathf.Sqrt(dX * dX + dY * dY);

                    var square = new Vector2Int(i, j);

                    if (grid.IsInBounds(square))
                    {
                        alpha = 0.5f * (1f - dst);
                        Color hClr = grid.IsOccupied(square)
                          ? grid.highlightNegative.WithAlpha(alpha)
                          : grid.IsSupported(square) 
                            ? grid.highlightPositive.WithAlpha(alpha)
                            : grid.highlightNeutral.WithAlpha(alpha);   

                        grid.Highlight(square, hClr);
                    }
                }
            }
        }
    }
}

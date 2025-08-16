using Animocity.Cities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Animocity.UI
{
    public class ControlContext_Demolition : ControlContext
    {
        private const int HIGHLIGHT_DIST = 1;
        public ControlContext_Demolition()
        {

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
            HashSet<Building> toHighlight = new();
            if (drag)
            {
                var demolishSquares = grid.GetSquaresBetween(dragFrom, positionWorld, Vector2Int.one, false);
                
                foreach(var sq in demolishSquares)
                {
                    if (grid.TryGetBuildingAt(sq, out var building))
                    {
                        if (building.Blue.canBeDemolished)
                        {
                            toHighlight.Add(building);
                        }
                    }
                    else
                    {
                        grid.Highlight(sq, grid.highlightNegative.WithAlpha(0.25f));
                    }
                }
            }
            else
            {
                var sq = grid.WorldToCell(positionWorld);
                if (grid.TryGetBuildingAt(sq, out var building))
                {
                    if (building.Blue.canBeDemolished)
                    {
                        toHighlight.Add(building);
                    }
                }
                else
                {
                    HighlightLocalGrid(grid, sq);
                }
            }
            foreach (Building building in toHighlight)
            {
                foreach (var tile in building.Blue.tilesNeeded)
                {
                    grid.Highlight(building.GridLocation + tile, grid.highlightNegative.WithAlpha(0.5f));
                }
            }
        }

        public override void OnInteract(CityGrid grid, Vector3 positionWorld, bool drag, Vector3 dragStartWorldPos)
        {
            HashSet<Building> toDemolish = new();
            if (drag)
            {
                var demolishSquares = grid.GetSquaresBetween(dragStartWorldPos, positionWorld, Vector2Int.one, false);
                foreach (var sq in demolishSquares)
                {
                    if(grid.TryGetBuildingAt(sq, out var building))
                    {
                        if (building.Blue.canBeDemolished)
                        {
                            toDemolish.Add(building);
                        }
                    }
                }
            }
            else
            {
                var sq = grid.WorldToCell(positionWorld);
                if (grid.TryGetBuildingAt(sq, out var building))
                {
                    if (building.Blue.canBeDemolished)
                    {
                        toDemolish.Add(building);
                    }
                }
            }
            foreach(Building killding in toDemolish)
            {
                killding.DemolishSelf(toDemolish);
            }
        }

        public override void OnInspect(CityGrid grid, Vector3 positionWorld)
        {
            Release();
        }

   
        private void HighlightLocalGrid(CityGrid grid, Vector2Int ctr)
        {

            var minCell = ctr - HIGHLIGHT_DIST*Vector2Int.one;
            float alpha;

            for (int i = minCell.x; i <= minCell.x + 2 * HIGHLIGHT_DIST; i++)
            {
                for (int j = minCell.y; j <= minCell.y + 2 * HIGHLIGHT_DIST; j++)
                {
                    float dX = (minCell.x + HIGHLIGHT_DIST - i) * 1f / (0.25f + HIGHLIGHT_DIST);
                    float dY = (minCell.y + HIGHLIGHT_DIST - j) * 1f / (0.25f + HIGHLIGHT_DIST);
                    float dst = Mathf.Sqrt(dX * dX + dY * dY);

                    var square = new Vector2Int(i, j);

                    if (grid.IsInBounds(square))
                    {
                        //MonoBehaviour.print($"Centre at {ctr}");
                        alpha = 0.5f * (1f - dst);
                        Color hClr = grid.highlightNegative.WithAlpha(alpha);
                        grid.Highlight(square, hClr);
                    }
                }
            }
        }
    }
}

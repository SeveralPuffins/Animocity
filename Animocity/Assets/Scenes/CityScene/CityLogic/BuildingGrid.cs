using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Animocity.Cities
{
    public class BuildingGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private List<Vector2> polygonPoints;
        public Polygon bounds { get; private set; }
        private Dictionary<Vector2Int, Building> tileContents;
        public Vector2 cellSize;
        [Header("Highlight Style")]
        public GridHighlightManager ghm;
        public Color highlightPositive = new Color(0.2f, 0.2f, 1f, 1f);
        public Color highlightNegative = new Color(1f, 0.2f, 0.2f, 1f);
        public Color highlightNeutral = new Color(0.6f, 0.6f, 0.65f, 1f);

        private void Awake()
        {
            bounds = new Polygon(polygonPoints);
            tileContents = new();

            var context = new ControlContext_Inspector();
            context.Activate();
        }
        private void OnValidate()
        {
            bounds = new Polygon(polygonPoints);
            cellSize.x = Math.Max(cellSize.x, 0.05f);
            cellSize.y = Math.Max(cellSize.y, 0.05f);
        }

        public bool IsInBounds(Vector2Int tile)
        {
            return bounds.Contains(new Vector2(tile.x*cellSize.x, tile.y*cellSize.y));
        }
        public bool IsOccupied(Vector2Int tile)
        {
            return tileContents.ContainsKey(tile);
        }
        public bool IsSupported(Vector2Int tile)
        {
            if (!IsInBounds(tile)) return false;

            var under = (tile + Vector2Int.down);

            if (!IsInBounds(under)) return true;

            if(TryGetBuildingAt(under, out var building))
            {
                return building.Blue.grantsSupport;
            }
            return false;
        }
        public bool TryGetBuildingAt(Vector2Int tile, out Building building)
        {
            return tileContents.TryGetValue(tile, out building);
        }

        private void PushBuilding(Building building)
        {
            foreach(var offset in building.Blue.tilesNeeded)
            {
                tileContents.Add(offset+building.GridLocation, building);
            }
        }

        public bool TryBuildAtLocation(BuildingBlueprint blue, Vector2Int loc, out Building newBuilding)
        {
            if(blue.CanBuildAtLocation(loc, this))
            {
                var newBuildingTransform = Instantiate<Transform>(blue.GetPrefab(), WorldFromCell(loc), Quaternion.identity);
                newBuildingTransform.SetParent(this.transform);
                newBuilding = Building.AddToGameObject(newBuildingTransform.gameObject, blue, loc);

                PushBuilding(newBuilding);
                return true;
            }
            newBuilding = null;
            return false;
        }

        private Vector2 GetMousePosition()
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            return (Vector2)mouseRay.GetPoint(1f*(this.transform.position-Camera.main.transform.position).z);

        }

        private Vector2Int GetMouseCell()
        {
            return WorldToCell(GetMousePosition());
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                ControlContext.Current.OnInteract(this, GetMousePosition());
            }
            else if(Input.GetMouseButtonDown(1)) {
                ControlContext.Current.OnInspect(this, GetMousePosition());
            }
            else
            {
                ControlContext.Current.OnHover(this, GetMousePosition());
            }
        }

        #region Grid Overlay Code
        
        void OnDrawGizmos()
        {

        #if UNITY_EDITOR
            DrawMapBoundary();
            DrawGrid();
        #endif
        }

        private void DrawMapBoundary()
        {
            Gizmos.color = new Color(0,1,1,0.5f);

            var drawPoints = new Vector3[polygonPoints.Count()];
            for (int i = 0; i < drawPoints.Count(); i++)
            {
                drawPoints[i] = transform.position + (Vector3)polygonPoints[i];
            }

            Gizmos.DrawLineStrip(drawPoints, true);
            Gizmos.color = Color.white;
        }

        private void DrawGrid()
        {

            var minCell = WorldToCell(bounds.BoundingBox.min + (Vector2)transform.position);
            var size = bounds.BoundingBox.size;
            
            for(int i = minCell.x; i< minCell.x+size.x; i++)
            {
                for(int j = minCell.y; j< minCell.y+size.y; j++)
                {
                    Vector2 cellLocation = new Vector2(i * cellSize.x, j * cellSize.y);
                    if (bounds.Contains(cellLocation))
                    {
                        DrawCellGizmo(WorldFromCell(i,j));
                    }
                }
            }
        }

        public void Highlight(Vector2Int gridSquare, Color clr)
        {
            if (!IsInBounds(gridSquare)) return;

            Vector2 min = WorldFromCell(gridSquare);
            Vector2 max = WorldFromCell(gridSquare + Vector2Int.one);

            Rect hr = new Rect(min, max - min);

            ghm.PushHighlight(new RectHighlight(hr, clr));
        }


        private void DrawCellGizmo(Vector2 ctr)
        {
            Gizmos.color = new Color(1f,1f,1f,0.3f);

            var points = new Vector3[4]
            {
                new Vector3(ctr.x - 0.5f * cellSize.x, ctr.y - 0.5f * cellSize.y, 0),
                new Vector3(ctr.x + 0.5f * cellSize.x, ctr.y - 0.5f * cellSize.y, 0),
                new Vector3(ctr.x + 0.5f * cellSize.x, ctr.y + 0.5f * cellSize.y, 0),
                new Vector3(ctr.x - 0.5f * cellSize.x, ctr.y + 0.5f * cellSize.y, 0)
            };

            Gizmos.DrawLineStrip(points, true);
        }
        #endregion

        #region Converting to and from cell co-ordinates
        public Vector2Int WorldToCell(Vector2 worldLocation)
        {
            var relative = worldLocation - (Vector2)transform.position;
            return new Vector2Int((int)Math.Round(relative.x / cellSize.x), (int)Math.Round(relative.y / cellSize.y));
        }
        public Vector2Int WorldToCell(float wx, float wy)
        {
            var relative = (new Vector2(wx - transform.position.x, wy - transform.position.y));
            return new Vector2Int((int)(relative.x / cellSize.x), (int)(relative.y / cellSize.y));
        }
        public Vector2 WorldFromCell(Vector2Int cell)
        {
            var x = cell.x * cellSize.x + transform.position.x;
            var y = cell.y * cellSize.y + transform.position.y;

            return new Vector2(x, y);
        }
        public Vector2 WorldFromCell(int i, int j)
        {
            return WorldFromCell(new Vector2Int(i, j));
        }
        public Vector2 WorldFromCell(float x, float y)
        {
            var wx = x * cellSize.x + transform.position.x;
            var wy = y * cellSize.y + transform.position.y;

            return new Vector2(wx, wy);
        }


        #endregion
    }
}

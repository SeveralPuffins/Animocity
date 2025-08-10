using Animocity.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Animocity.Cities
{
    public class CityGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private List<Vector2> polygonPoints;
        public PowerGrid powerGrid;
        public TransportGrid transportGrid;
        public List<string> gridTags;
        public Polygon bounds { get; private set; }
        private Dictionary<Vector2Int, Building> tileContents;
        public Vector2 cellSize;
        [Header("Highlight Style")]
        public GridHighlightManager ghm;
        public Color highlightPositive = new Color(0.2f, 0.2f, 1f, 1f);
        public Color highlightNegative = new Color(1f, 0.2f, 0.2f, 1f);
        public Color highlightNeutral = new Color(0.6f, 0.6f, 0.65f, 1f);
        private bool _focused = false;

        public void Focus()
        {
            _focused = true;
        }
        public void Unfocus()
        {
            _focused = false;
        }

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
        public RectInt TileBounds
        {
            get
            {
                return new RectInt(
                            (int)(bounds.BoundingBox.xMin   / cellSize.x), 
                            (int)(bounds.BoundingBox.yMin   / cellSize.y),
                            (int)(bounds.BoundingBox.width  / cellSize.x), 
                            (int)(bounds.BoundingBox.height / cellSize.y)
                        );
            }
        }

        public List<Vector2Int> GetBaseTiles()
        {
            List<Vector2Int> tiles = new List<Vector2Int>();
            for(float fi = bounds.BoundingBox.xMin; fi <= bounds.BoundingBox.xMax; fi += cellSize.x)
            {
                for (float fj = bounds.BoundingBox.yMin; fj <= bounds.BoundingBox.yMax; fj += cellSize.y)
                {
                    var psn = new Vector2(fi, fj);
                    if (bounds.Contains(psn))
                    {
                        //MonoBehaviour.print($"Adding base tile position {psn}");
                        var intPsn = new Vector2Int((int)(fi/cellSize.x), (int)(fj/cellSize.y));
                        tiles.Add(intPsn);
                        break;
                    }
                }
            }

           return tiles;
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
        private List<Vector2Int> GetSquares(Vector2Int fromSquare, Vector2Int toSquare, Vector2Int stride, bool lockLongestRowCol = false)
        {
            int roundedToSquareX = ((toSquare.x - fromSquare.x) / stride.x)*stride.x + fromSquare.x;
            int roundedToSquareY = ((toSquare.y - fromSquare.y) / stride.y)*stride.y + fromSquare.y;

            int iMin = Math.Min(roundedToSquareX, fromSquare.x);
            int jMin = Math.Min(roundedToSquareY, fromSquare.y);

            int iMax = Math.Max(toSquare.x, fromSquare.x);
            int jMax = Math.Max(toSquare.y, fromSquare.y);

            int dx = iMax - iMin;
            int dy = jMax - jMin;

            if (dx == 0 && dy == 0)
            {
                return new List<Vector2Int>() { fromSquare };
            }

            if (lockLongestRowCol)
            {
                if (dx >= dy)
                {
                    jMin = jMax = fromSquare.y;
                }
                else
                {
                    iMin = iMax = fromSquare.x;
                }
            }

            List<Vector2Int> squaresList = new();
            for (int i = iMin; i<=iMax; i+=stride.x)
            {
                for(int j = jMin; j<=jMax; j+=stride.y)
                {
                    squaresList.Add(new Vector2Int(i, j));
                }
            }
            return squaresList;
        }
        public List<Vector2Int> GetSquaresBetween(Vector3 dragFromPosition, Vector3 dragToPosition, Vector2Int stride, bool lockLongestRowCol = false)
        {
            var fromSquare = WorldToCell(dragFromPosition);
            var toSquare = WorldToCell(dragToPosition);
            return GetSquares(fromSquare, toSquare, stride, lockLongestRowCol);
        }

        public bool TryGetBuildingAt(Vector2Int tile, out Building building)
        {
            return tileContents.TryGetValue(tile, out building);
        }

        private void PushBuilding(Building building)
        {
            foreach(var offset in building.Blue.tilesNeeded)
            {
                tileContents.Remove(offset + building.GridLocation);
                tileContents.Add(offset+building.GridLocation, building);
            }
        }

        public bool TryBuildAtLocation(BuildingBlueprint blue, Vector2Int loc, out Building newBuilding, bool isFree = false)
        {
            if (!IsInBounds(loc))
            {
                newBuilding = null;
                return false;
            }

            if(blue.CanBuildAtLocation(loc, this))
            {
                bool isPlan = !(CanAfford(blue) || isFree);

                var newBuildingTransform = Instantiate<Transform>(blue.GetPrefab(), WorldFromCell(loc), Quaternion.identity);
                newBuildingTransform.SetParent(this.transform);
                newBuilding = Building.AddToGameObject(newBuildingTransform.gameObject, blue, this, loc, isPlan);

                PushBuilding(newBuilding);
                if(!isFree && !isPlan) PayResources(blue, loc);

                return true;
                
            }
            newBuilding = null;
            return false;
        }

        private void PayResources(BuildingBlueprint blue, Vector2Int tile)
        {
            foreach (var key in blue.resourceCosts.Keys) 
            {
                CityOverview.Current.TakeResource(tile, key, blue.resourceCosts[key]);
            }
        }

        private bool CanAfford(BuildingBlueprint blue)
        {
            return CityOverview.Current.HasResources(blue.resourceCosts);
        }

        private Vector3 GetMousePosition()
        {
            var pt =  Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 3.5f));
            var offset = (Vector3)cellSize * 0.5f;

            return pt - offset;
            //Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            //return (Vector2)mouseRay.GetPoint(1f*(this.transform.position-Camera.main.transform.position).z);
        }

        private Vector2Int GetMouseCell()
        {
            return WorldToCell(GetMousePosition());
        }

        bool _dragging = false;
        float _minDragDist = 0.2f;
        Vector2 _dragStartLocation;

        private void CheckMouseInteractions()
        {
            var pos = GetMousePosition();
            if (Input.GetMouseButtonDown(0))
            {
                _dragging = true;
                _dragStartLocation = pos;
            }

            var drag = _dragging && (((Vector2)pos - _dragStartLocation).sqrMagnitude >= _minDragDist * _minDragDist);

            if (Input.GetMouseButtonUp(0))
            {
                ControlContext.Current.OnInteract(this, pos, drag, _dragStartLocation);
                _dragging = false;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ControlContext.Current.OnInspect(this, pos);
                _dragging = false;
            }
            else
            {
                ControlContext.Current.OnHover(this, pos, drag, _dragStartLocation);
            }
        }

        public void Update()
        {
            if (_focused) { 
               CheckMouseInteractions();
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

            var minCell = WorldToCell(transform.position+new Vector3(bounds.BoundingBox.min.x, bounds.BoundingBox.min.y, 0f));
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

            ghm.PushHighlight(new RectHighlight(hr, clr, this.transform.position.z));
        }


        private void DrawCellGizmo(Vector2 ctr)
        {
            Gizmos.color = new Color(1f,1f,1f,0.3f);

            var points = new Vector3[4]
            {
                new Vector3(ctr.x - 0.5f * cellSize.x, ctr.y - 0.5f * cellSize.y, transform.position.z),
                new Vector3(ctr.x + 0.5f * cellSize.x, ctr.y - 0.5f * cellSize.y, transform.position.z),
                new Vector3(ctr.x + 0.5f * cellSize.x, ctr.y + 0.5f * cellSize.y, transform.position.z),
                new Vector3(ctr.x - 0.5f * cellSize.x, ctr.y + 0.5f * cellSize.y, transform.position.z)
            };

            Gizmos.DrawLineStrip(points, true);
        }
        #endregion

        #region Converting to and from cell co-ordinates
        public Vector2 WorldToFloatCell(Vector3 worldLocation)
        {
            var relative = worldLocation - transform.position;
            return new Vector2Int((int)Math.Round(relative.x / cellSize.x), (int)Math.Round(relative.y / cellSize.y));
        }
        public Vector2Int WorldToCell(Vector3 worldLocation)
        {
            var relative = worldLocation - transform.position;
            return new Vector2Int((int)Math.Round(relative.x / cellSize.x), (int)Math.Round(relative.y / cellSize.y));
        }

        public Vector2Int WorldToCell(float wx, float wy)
        {
            var relative = (new Vector2(wx - transform.position.x, wy - transform.position.y));
            return new Vector2Int((int)(relative.x / cellSize.x), (int)(relative.y / cellSize.y));
        }
        public Vector3 WorldFromCell(Vector2Int cell)
        {
            var x = cell.x * cellSize.x + transform.position.x;
            var y = cell.y * cellSize.y + transform.position.y;

            return new Vector3(x, y, this.transform.position.z);
        }
        public Vector2 WorldFromCell(int i, int j)
        {
            return WorldFromCell(new Vector2Int(i, j));
        }
        public Vector3 WorldFromCell(float x, float y)
        {
            var wx = x * cellSize.x + transform.position.x;
            var wy = y * cellSize.y + transform.position.y;

            return new Vector3(wx, wy, transform.position.z);
        }


        #endregion
    }
}

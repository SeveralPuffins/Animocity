using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Animocity.Cities.Algorithms;

namespace Animocity.Cities
{
    public class MultiGrid  //: IGraphHeuristic
    {
        private List<CityGrid> activeGrids;
        private List<GridConnector> bridges = new();

        private int _focusedGrid = 0;
        public CityGrid FocusedGrid
        {
            get
            {
                return activeGrids[_focusedGrid];
            }
        }

        private void UnfocusCurrent()
        {
            FocusedGrid.Unfocus();
        }
        private void FocusCurrent()
        {
            FocusedGrid.Focus();
        }

        public void FocusNext()
        {
            UnfocusCurrent();
            _focusedGrid = (_focusedGrid + 1) % activeGrids.Count();
            FocusCurrent();
        }
        public void FocusPrevious()
        {
            UnfocusCurrent();
            _focusedGrid = ((_focusedGrid - 1) + activeGrids.Count()) % activeGrids.Count();
            FocusCurrent();
        }

        public void Highlight(MultiPoint p, Color clr)
        {
            GetGrid(p.GridIndex).Highlight(p.Coords, clr);
        }

        public Graph<MultiPoint> GraphFromMultipoints(Dictionary<MultiPoint, float> graphPoints)
        {
            var adjacencies = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            Dictionary<MultiPoint, List<MultiPoint>> temp_edges = new();
            Dictionary<MultiPoint, List<float>> temp_edgeCosts = new();

            List<MultiPoint> edgesForSquare;
            List<float> costsForSquare;

            foreach (var grid in graphPoints.Keys.GroupBy(mp => mp.GridIndex))
            {
                int idx = grid.Key;

                foreach (MultiPoint square in grid)
                {
                    edgesForSquare = new();
                    costsForSquare = new();

                    foreach (var adjacency in adjacencies)
                    {
                        var target = new MultiPoint(square.Coords + adjacency, square.GridIndex);

                        if (graphPoints.TryGetValue(target, out var cost))
                        {
                            edgesForSquare.Add(target);
                            costsForSquare.Add(cost);
                        }
                    }
                    if (edgesForSquare.Count > 0)
                    {
                        temp_edges.Add(square, edgesForSquare);
                        temp_edgeCosts.Add(square, costsForSquare);
                    }
                }
            }

            foreach(var connector in bridges)
            {
                if(graphPoints.ContainsKey(connector.ConnectionEndA))
                {
                    if (graphPoints.ContainsKey(connector.ConnectionEndB))
                    {
                        if (!temp_edges.ContainsKey(connector.ConnectionEndA))
                        {
                            temp_edges.Add(connector.ConnectionEndA, new());
                            temp_edgeCosts.Add(connector.ConnectionEndA, new());
                        }
                        if (!temp_edges.ContainsKey(connector.ConnectionEndB))
                        {
                            temp_edges.Add(connector.ConnectionEndB, new());
                            temp_edgeCosts.Add(connector.ConnectionEndB, new());
                        }

                        temp_edges[connector.ConnectionEndA].Add(connector.ConnectionEndB);
                        temp_edgeCosts[connector.ConnectionEndA].Add(connector.ConnectionTransitCost);

                        temp_edges[connector.ConnectionEndB].Add(connector.ConnectionEndA);
                        temp_edgeCosts[connector.ConnectionEndB].Add(connector.ConnectionTransitCost);
                    }
                } 
            }
            Dictionary<MultiPoint, MultiPoint[]> edges = new();
            Dictionary<MultiPoint, float[]> edgeCosts = new();
            foreach (var mp in temp_edges.Keys)
            {
                edges.Add(mp, temp_edges[mp].ToArray()); 
                edgeCosts.Add(mp, temp_edgeCosts[mp].ToArray());
            }

            var gridGraph = new Graph<MultiPoint>(edges, edgeCosts);

            return gridGraph;
        }

        public void AddGrid(CityGrid grid)
        {
            if(activeGrids == null) activeGrids = new();

            this.activeGrids.Add(grid);
        }
        public bool TryAddGridConnection(CityGrid grid1, Vector2Int position1, CityGrid grid2, Vector2Int position2, float transitCost)
        {
            if (bridges == null) bridges = new();

            if(activeGrids==null) return false;
            if (!activeGrids.Contains(grid1)) return false;
            if (!activeGrids.Contains(grid2)) return false;

            if(grid1.IsInBounds(position1) && grid2.IsInBounds(position2))
            {
                int idx1 = activeGrids.IndexOf(grid1);
                int idx2 = activeGrids.IndexOf(grid2);

                bridges.Add(new GridConnector(idx1, idx2, position1, position2 , transitCost));
            }
            return false;
        }

        public CityGrid GetGrid(int n)
        {
            if (n < 0 || n >= this.activeGrids.Count)
            {
                return null;
            }
            else return activeGrids[n];
        }
        public CityGrid[] GetAllGrids()
        {
            return activeGrids.ToArray();
        }
        public int GetIndex(CityGrid grid)
        {
            return activeGrids.IndexOf(grid);
        }

        internal void BuildBridges()
        {
            for(int i=0; i<activeGrids.Count()-1; i++)
            {
                for(int j=i+1; j<activeGrids.Count(); j++)
                {
                    var gi = GetGrid(i);
                    var gj = GetGrid(j);

                    if (gi.externalConnections.Contains(gj))
                    {
                        if (gj.externalConnections.Contains(gi))
                        {
                            int gix = gi.externalConnections.IndexOf(gj);
                            int gjx = gj.externalConnections.IndexOf(gi);

                            var giPos = gi.externalConnectionPoints[gix];
                            var gjPos = gj.externalConnectionPoints[gjx];

                            bridges.Add(new GridConnector(i, j, giPos, gjPos, 0.5f));
                        }
                    }
                }
            }
        }
    }
}

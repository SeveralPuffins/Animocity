using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Animocity.Cities.Algorithms
{
    public class Graph<T>
    {
        public Graph() { }

        private Dictionary<T, T[]> _edges;
        private Dictionary<T, float[]> _edgeCosts;

        /// <summary>
        /// Tries to find paths from start to all of the T ends. Because there is no one set direction to this,
        /// it uses Dijkstra's rather than A* / HAstar
        /// Returns true if it funds any. 
        /// </summary>
        /// <param name="start">The start location for the paths</param>
        /// <param name="ends">The list of end locations to find paths to</param>
        /// <param name="paths">The paths are returned in ascending cost order here.</param>
        /// <returns></returns>
        public bool TryFindPaths(T start, IEnumerable<T> ends, out List<Path<T>> paths, float maxCost = float.MaxValue)
        {
            paths = new List<Path<T>>();
            if (_edges == null || _edges.Count() == 0)
            {
                return false;
            }

            Dictionary<T, float> minDistances = new Dictionary<T, float>();

            int size = (int) Math.Pow(2, Math.Ceiling(Math.Log(_edges.Count(), 2)));


            AffinityColumn<T> minDistanceHeap = new(size);
            Dictionary<T, T> parents = new();
            minDistances.Add(start, 0f);
            minDistanceHeap.Add(start, 0f);



            while (paths.Count() < ends.Count() && minDistanceHeap.Count > 0)
            {
                var current = minDistanceHeap.Pop(out var currentCost);
                if (currentCost > maxCost) break;

                // Terminate early if all ends have been discovered and if the minimum distance to an end is less than the heap minimum
                // because this means that the cheapest way of getting to an unexplored node is already more expensive than getting to 
                // each end point
                if (ends.All((end) => minDistances.ContainsKey(end)))
                {
                    float mostExpensiveEnd = ends.Max(end => minDistances[end]);

                    if (mostExpensiveEnd < currentCost) break;
                }



                if (!_edges.TryGetValue(current, out var currentEdges)) continue;

                for(int i = 0; i < currentEdges.Count(); i++)
                {
                    var edgeNode = currentEdges[i];
                    var edgeCost = _edgeCosts[current][i] + currentCost;

                    
                    if (!minDistances.TryGetValue(edgeNode, out var oldEdgeCost) 
                    ||   oldEdgeCost > edgeCost)
                    { 
                        
                        minDistances[edgeNode] = edgeCost;
                        parents.Add(edgeNode, current);
                        minDistanceHeap.UpdateValue(edgeNode, edgeCost);
                        
                    }
                }
            }

            foreach(var end in ends)
            {
                if (minDistances.TryGetValue(end, out float cost)){
                    paths.Add(TraversePath(end, parents, minDistances));
                }
            }

            paths = paths.OrderBy((p) => p.TotalCost).ToList();

            return paths.Count() > 0;
        }

        private static Path<T> TraversePath(T end, Dictionary<T, T> parents, Dictionary<T, float> minCosts)
        {
            var nodes = new List<T>();
            var costs = new List<float>();

            var current = end;
            nodes.Add(current);

            while(parents.TryGetValue(current, out var parent))
            {
                costs.Add(minCosts[current] - minCosts[parent]);
                current = parent;
                nodes.Add(current);
            }
            costs.Add(0f);

            nodes.Reverse();
            costs.Reverse();

            return new Path<T>(nodes, costs);
        }


        public static Graph<Vector2Int> FromGridSquares(Dictionary<Vector2Int, float> grid)
        {
            var adjacencies = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            Dictionary<Vector2Int, Vector2Int[]> edges = new();
            Dictionary<Vector2Int, float[]> edgeCosts = new();

            List<Vector2Int> edgesForSquare;
            List<float> costsForSquare;

            foreach (var square in grid.Keys) {
                edgesForSquare = new();
                costsForSquare = new();

                foreach (var adjacency in adjacencies)
                {
                    var target = square + adjacency;

                    if (grid.TryGetValue(target, out var cost))
                    {
                        edgesForSquare.Add(target);
                        costsForSquare.Add(cost);
                    }
                }
                if (edgesForSquare.Count > 0)
                {
                    edges.Add(square, edgesForSquare.ToArray());
                    edgeCosts.Add(square, costsForSquare.ToArray());
                }
            }

            var gridGraph = new Graph<Vector2Int>();
            gridGraph._edgeCosts = edgeCosts;
            gridGraph._edges = edges;

            return gridGraph;
        }
    }
}

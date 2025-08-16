using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.SearchService;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Animocity.Cities.Algorithms
{
    public class Graph<T>
    {
        public Graph() 
        {
            _edgeCosts = new();
            _edges = new();
            _cachedQueries = new();
        }


        private Dictionary<PathQuery<T>, List<Path<T>>> _cachedQueries;
        private Dictionary<T, T[]> _edges;
        private Dictionary<T, float[]> _edgeCosts;

        public void ClearCache()
        {
            this._cachedQueries.Clear();
        }

        /// <summary>
        /// Tries to find paths from start to all of the T ends. Because there is no one set direction to this,
        /// it uses Dijkstra's rather than A* / HAstar
        /// Returns true if it funds any. 
        /// </summary>
        /// <param name="start">The start location for the paths</param>
        /// <param name="ends">The list of end locations to find paths to</param>
        /// <param name="paths">The paths are returned in ascending cost order here.</param>
        /// <param name="maxDistance"> Paths can be at most maxDistance cost..</param>
        /// <returns></returns>
        /// 
        public bool TryFindPaths(T start, IEnumerable<T> ends, out List<Path<T>> paths, float maxDistance)
        {
            var query = new PathQuery<T>(start, ends, maxDistance);
            return TryFindPaths(query, out paths);
        }
        public bool TryFindPaths(T startLocation, IEnumerable<T> endpoints, out List<Path<T>> paths)
        {
            var query = new PathQuery<T>(startLocation, endpoints);
            return TryFindPaths(query, out paths);
        }


        private bool TryFindPaths(PathQuery<T> query, out List<Path<T>> paths)
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
            minDistances.Add(query.start, 0f);
            minDistanceHeap.Add(query.start, 0f);

            /*if (!_edges.Keys.Contains(query.start))
            {
                MonoBehaviour.print($"START LOCATION NOT IN EDGES!");
            }
            foreach (var end in query.endpoints)
            {
                if (!_edges.Any((edge)=>edge.Value.Contains(end)))
                {
                    MonoBehaviour.print($"AN END LOCATION NOT IN EDGES!");
                }
            }*/

            //MonoBehaviour.print($"Trying to find path from {query.start} to up to {query.endpoints.Count()} ends, in a graph with {_edges.Count()} edges.");


            while (paths.Count() < query.endpoints.Count() && minDistanceHeap.Count > 0)
            {
                var current = minDistanceHeap.Pop(out var currentCost);

                //MonoBehaviour.print($"Iterating from {current} with cost {currentCost}.");

                if (currentCost > query.maxCost) break;

                // Terminate early if all ends have been discovered and if the minimum distance to an end is less than the heap minimum
                // because this means that the cheapest way of getting to an unexplored node is already more expensive than getting to 
                // each end point
                if (query.endpoints.All((end) => minDistances.ContainsKey(end)))
                {
                    float mostExpensiveEnd = query.endpoints.Max(end => minDistances[end]);

                    if (mostExpensiveEnd < currentCost)
                    {
                        //MonoBehaviour.print($"All ends found, most expensive end costs {mostExpensiveEnd}, less than next min cost {currentCost}.");
                        break;
                    }
              
                }



                if (!_edges.TryGetValue(current, out var currentEdges)) continue;

                //MonoBehaviour.print($"Iterating {currentEdges.Count()} linkes from current node.");
                for (int i = 0; i < currentEdges.Count(); i++)
                {
                    var edgeNode = currentEdges[i];
                    var edgeCost = _edgeCosts[current][i] + currentCost;

                    
                    if (!minDistances.TryGetValue(edgeNode, out var oldEdgeCost))
                    {
                        //MonoBehaviour.print($"Updating cost to get to {currentEdges[i]} to {edgeCost}.");
                        minDistances[edgeNode] = edgeCost;
                        parents.Add(edgeNode, current);
                        minDistanceHeap.Add(edgeNode, edgeCost);
                        
                    }
                    else if(oldEdgeCost > edgeCost)
                    {
                        //MonoBehaviour.print($"Updating cost to get to {currentEdges[i]} to {edgeCost}.");
                        minDistances[edgeNode] = edgeCost;
                        parents.Add(edgeNode, current);
                        minDistanceHeap.UpdateValue(edgeNode, edgeCost);
                    }
                }
            }

            foreach(var end in query.endpoints)
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

        internal bool TryCheckRoute(List<T> route, out float cost)
        {
            cost = 0;
            for(int i=0; i<route.Count-1; i++)
            {
                T o = route[i];
                T d = route[i+1];

                if (_edges.TryGetValue(o, out var edgeList))
                {
                    bool foundEdge = false;
                    for(int n=0; n<edgeList.Length; n++)
                    {
                        if (edgeList[n].Equals(d))
                        {
                            foundEdge = true;
                            cost += _edgeCosts[o][n];
                            break;
                        }
                    }
                    if (!foundEdge)
                    {
                        return false;
                    }
                }
                else return false;
            }
            return true;
        }

        public HashSet<T> FindAllInRange(T startNode, float maxCost)
        {
            HashSet<T> nodes = new HashSet<T>();

            if (_edges == null || _edges.Count() == 0)
            {
                return nodes;
            }

            Dictionary<T, float> minDistances = new Dictionary<T, float>();
            int size = (int)Math.Pow(2, Math.Ceiling(Math.Log(_edges.Count(), 2)));

            AffinityColumn<T> minDistanceHeap = new(size);
            minDistances.Add(startNode, 0f);
            minDistanceHeap.Add(startNode, 0f);

            if (!_edges.Keys.Contains(startNode))
            {
                MonoBehaviour.print($"START LOCATION NOT IN EDGES!");
            }
        


            while (minDistanceHeap.Count > 0)
            {
                var current = minDistanceHeap.Pop(out var currentCost);

                if (currentCost > maxCost) break;
                if (!_edges.TryGetValue(current, out var currentEdges)) continue;

                for (int i = 0; i < currentEdges.Count(); i++)
                {
                    var edgeNode = currentEdges[i];
                    var edgeCost = _edgeCosts[current][i] + currentCost;

                    if (!minDistances.TryGetValue(edgeNode, out var oldEdgeCost))
                    {
                        //MonoBehaviour.print($"Adding {currentEdges[i]} at cost {edgeCost}.");
                        minDistances[edgeNode] = edgeCost;
                        minDistanceHeap.Add(edgeNode, edgeCost);

                    }
                    else if (oldEdgeCost > edgeCost)
                    {
                        //MonoBehaviour.print($"Updating {currentEdges[i]} to cost {edgeCost} from {current}.");
                        minDistances[edgeNode] = edgeCost;
                        minDistanceHeap.UpdateValue(edgeNode, edgeCost);
                    }
                }
            }
            foreach(T node in minDistances.Keys)
            {
                if (minDistances[node] < maxCost)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }
    }
}

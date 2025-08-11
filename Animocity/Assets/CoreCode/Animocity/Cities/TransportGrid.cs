using Animocity.Cities.Algorithms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animocity.Cities
{
    public class TransportGrid
    {
        private Graph<Vector2Int> graph;
        private Dictionary<Vector2Int, float> _gridSquares;
        private Dictionary<Vector2Int, float> _newGridSquares;
        private List<Vector2Int> _removedGridSquares;

        public TransportGrid() 
        {
            _gridSquares = new();
            _newGridSquares = new();
            _removedGridSquares = new();
        }

        public void AddSquare(Vector2Int square, float cost)
        {
            _newGridSquares.Add(square, cost);
        }

        private void UpdateGrid()
        {
            bool changed = false;
            if(_removedGridSquares.Count > 0)
            {
                foreach(var s in _removedGridSquares)
                {
                    _gridSquares.Remove(s);
                }
                _removedGridSquares.Clear();
                changed = true;
            }

            if (_newGridSquares.Count > 0)
            {
                foreach(var square in _newGridSquares.Keys)
                {
                    _gridSquares[square] = _newGridSquares[square];
                }
                _newGridSquares.Clear();
                UpdateGraphRepresentation();
                changed = true;
            }
            if (changed) graph.ClearCache();
        }

        private void UpdateGraphRepresentation()
        {
            this.graph = Graph<Vector2Int>.FromGridSquares(_gridSquares);
        }

        public bool TryFindPaths(Vector2Int startLocation, IEnumerable<Vector2Int> endpoints, float maxDistance, out List<Path<Vector2Int>> paths)
        {
            UpdateGrid();
            return graph.TryFindPaths(startLocation, endpoints, out paths, maxDistance);
        }

        public HashSet<Vector2Int> GetConnectedTiles(Vector2Int startLocation, float maxDistance)
        {
            UpdateGrid();
            return graph.FindAllInRange(startLocation, maxDistance);
        }

        internal void RemoveSquare(Vector2Int location)
        {
            _removedGridSquares.Add(location);
        }
    }
}

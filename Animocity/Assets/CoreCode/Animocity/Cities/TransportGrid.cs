using Animocity.Cities.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Animocity.Utilities;

namespace Animocity.Cities
{
    public class TransportGrid
    {
        private Graph<Vector2Int> graph;
        private Dictionary<Vector2Int, float> _gridSquares;
        private Dictionary<Vector2Int, float> _newGridSquares;

        public TransportGrid() 
        {
            _gridSquares = new();
            _newGridSquares = new();
        }

        public void AddSquare(Vector2Int square, float cost)
        {
            _newGridSquares.Add(square, cost);
        }

        private void UpdateGrid()
        {
            if (_newGridSquares.Count > 0)
            {
                foreach(var square in _newGridSquares.Keys)
                {
                    _gridSquares[square] = _newGridSquares[square];
                }
                _newGridSquares.Clear();
                UpdateGraphRepresentation();
            }
            graph.ClearCache();
        }

        private void UpdateGraphRepresentation()
        {
            this.graph = Graph<Vector2Int>.FromGridSquares(_gridSquares);
        }

        public bool TryFindPaths(Vector2Int startLocation, IEnumerable<Vector2Int> endpoints, float maxDistance, out List<Path<Vector2Int>> paths)
        {
            if(_newGridSquares.Count > 0)
            {
                UpdateGrid();
            }
            return graph.TryFindPaths(startLocation, endpoints, out paths, maxDistance);
        }
    }
}

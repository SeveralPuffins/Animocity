using Animocity.Cities.Algorithms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animocity.Cities
{
    public class TransportGrid
    {
        private MultiGrid grid;
        private Graph<MultiPoint> graph;
        private Dictionary<MultiPoint, float> _transportSquares;
        private Dictionary<MultiPoint, float> _newTransportSquares;
        private List<MultiPoint> _removedTransportSquares;

        public TransportGrid(MultiGrid grid) 
        {
            this.grid = grid;
            _transportSquares = new();
            _newTransportSquares = new();
            _removedTransportSquares = new();
        }

        public void AddSquare(MultiPoint square, float cost)
        {
            _newTransportSquares.Add(square, cost);
        }

        private void UpdateGrid()
        {
            bool changed = false;
            if (_removedTransportSquares.Count > 0)
            {
                foreach (var s in _removedTransportSquares)
                {
                    _transportSquares.Remove(s);
                }
                _removedTransportSquares.Clear();
                changed = true;
            }

            if (_newTransportSquares.Count > 0)
            {
                foreach (var square in _newTransportSquares.Keys)
                {
                    _transportSquares[square] = _newTransportSquares[square];
                }
                _newTransportSquares.Clear();
                changed = true;
            }
            if (changed)
            {
                UpdateGraphRepresentation();
                graph.ClearCache();
            }
        }

        private void UpdateGraphRepresentation()
        {
            this.graph = grid.GraphFromMultipoints(_transportSquares);
        }

        public bool TryFindPaths(MultiPoint startLocation, IEnumerable<MultiPoint> endpoints, float maxDistance, out List<Path<MultiPoint>> paths)
        {
            UpdateGrid();
            return graph.TryFindPaths(startLocation, endpoints, out paths, maxDistance);
        }

        public HashSet<MultiPoint> GetConnectedTiles(MultiPoint startLocation, float maxDistance)
        {
            UpdateGrid();
            return graph.FindAllInRange(startLocation, maxDistance);
        }

        internal void RemoveSquare(MultiPoint location)
        {
            _removedTransportSquares.Add(location);
        }

        internal bool TryCheckRoute(List<MultiPoint> route, out float cost)
        {
            return this.graph.TryCheckRoute(route, out cost);
        }
    }
}

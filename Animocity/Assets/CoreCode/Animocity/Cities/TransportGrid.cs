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
        private List<Vector2Int> _gridSquares;
        private List<Vector2Int> _newGridSquares;

        private TransportGrid() { }
        public TransportGrid(List<Vector2Int> memberSquares) 
        {
            _gridSquares = new();
            _newGridSquares = new();
            this.UpdateGrid();
        }

        public void AddSquare(Vector2Int square)
        {
            _newGridSquares.Add(square);
        }

        private void UpdateGrid()
        {
            if (_newGridSquares.Count > 0)
            {
                _gridSquares.AddRange(_newGridSquares);
                _newGridSquares.Clear();
                UpdateGraphRepresentation();
            }
        }

        private void UpdateGraphRepresentation()
        {
            var costs = new Dictionary<Vector2Int, float>();
            foreach(var loc in _gridSquares)
            { 
                
            }
            this.graph = Graph<Vector2Int>.FromGridSquares(costs);
        }

        public static TransportGrid MergeGrids(TransportGrid[] gridsToMerge)
        {
            List<Vector2Int> newMembers = new ();

            foreach (var grid in gridsToMerge)
            {
                newMembers.AddRange(grid._gridSquares);
            }

            return new TransportGrid(newMembers);
        }

        private Dictionary<BuildingComponent_PowerSupplier, float> supplyAvailable = new();
        

        /*
        public bool TryFindWorkers(BuildingComponent_PowerConsumer consumer)
        {
            if (!this._users.Contains(consumer)) return false;

            if(graph.TryFindPaths(
                start: consumer.Building.GridLocation,
                ends: Suppliers.Map((sup) => sup.Building.GridLocation),
                out var paths,
                1000f
            )){
                float demand = consumer.PowerData.powerConsumption;

                float checkSupply = demand;

                // First, confirm that there's enough power, given losses
                foreach(var path in paths)
                {
                    var supplier = getSupplierAt[path.Destination];

                    float lossMultiplier = 1f + path.TotalCost;
                    float available = Math.Min(lossMultiplier * checkSupply, supplyAvailable[supplier]);

                    checkSupply -= available/lossMultiplier;
                    if (checkSupply <= 0f) break;
                }
                if (checkSupply > 0) return false;

                // Now that we are sure the power is there, actually take that power from the grid.
                foreach (var path in paths)
                {
                    var supplier = getSupplierAt[path.Destination];

                    float lossMultiplier = 1f + path.TotalCost;
                    float available = Math.Min(lossMultiplier * demand, supplyAvailable[supplier]);

                    demand -= available / lossMultiplier;
                    if (demand <= 0f) break;
                }
                return true;
            }
            else return false;
        }*/
    }
}

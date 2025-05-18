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
    public class PowerGrid
    {
        public const float COST_LV = 0.01f;
        public const float COST_HV = 0.0001f;

        private Graph<Vector2Int> graph;

        private List<BuildingComponent_PowerConsumer> _users; 
        public IEnumerable<BuildingComponent_PowerConsumer> Users
        {
            get
            {
                return _users;
            }
        }

        private List<BuildingComponent_PowerSupplier> _suppliers;
        public IEnumerable<BuildingComponent_PowerSupplier> Suppliers 
        {
            get
            {
                return _suppliers;
            }
        }
        private Dictionary<Vector2Int, BuildingComponent_PowerSupplier> getSupplierAt = new();

        private List<BuildingComponent_PowerConsumer> _storage;
        public IEnumerable<BuildingComponent_PowerConsumer> Storage
        {
            get
            {
                return _storage;
            }
        }

        private List<Vector2Int> _lowVoltage;
        private List<Vector2Int> _highVoltage;

        private  PowerGrid() { }
        public PowerGrid(List<BuildingComponent_PowerSupplier> suppliers, List<BuildingComponent_PowerConsumer> users, List<Vector2Int> lowVoltageTiles, List<Vector2Int> highVoltageTiles) 
        {
            this._suppliers = suppliers;
            this._users = users;
            this._lowVoltage = lowVoltageTiles;
            this._highVoltage = highVoltageTiles;

            this.UpdateGridMembership();
            this.UpdateGraphRepresentation();
        }

        private void UpdateGridMembership()
        {
            foreach(var loc in _users)
            {
                loc.UpdateGrid(this);
            }
        }

        private void UpdateGraphRepresentation()
        {
            var costs = new Dictionary<Vector2Int, float>();

            foreach (var xy in _lowVoltage)
            {
                costs.Add(xy, COST_LV);
            }
            foreach (var xy in _highVoltage)
            {
                costs.Add(xy, COST_HV);
            }

            this.graph = Graph<Vector2Int>.FromGridSquares(costs);
        }

        private void UpdateSupplierLocations()
        {
            this.getSupplierAt.Clear();
            foreach(var supplier in _suppliers)
            {
                this.getSupplierAt.Add(supplier.Building.GridLocation, supplier);
            }
        }

        public static PowerGrid MergeGrids(PowerGrid[] gridsToMerge)
        {
            List<BuildingComponent_PowerConsumer> newUsers = new ();
            List<BuildingComponent_PowerSupplier> newSuppliers = new ();
            List<Vector2Int> newLowVoltage = new ();
            List<Vector2Int> newHighVoltage = new ();

            foreach (var grid in gridsToMerge)
            {
                newUsers.AddRange(grid.Users);
                newSuppliers.AddRange(grid.Suppliers);
                newLowVoltage.AddRange(grid._lowVoltage);
                newHighVoltage.AddRange(grid._highVoltage);
            }

            return new PowerGrid(newSuppliers, newUsers, newLowVoltage, newHighVoltage);
        }



        private Dictionary<BuildingComponent_PowerSupplier, float> supplyAvailable = new();
        

        public bool TryFindPower(BuildingComponent_PowerConsumer consumer)
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

                    checkSupply -= available/lossMultiplier;as
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
        }


        public void Resupply(BuildingComponent_PowerSupplier supplier)
        {
            supplyAvailable[supplier] = supplier.PowerData.powerConsumption;
        }

    }
}

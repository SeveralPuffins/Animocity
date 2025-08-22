using Animocity.Cities.Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Animocity.Utilities;
using UnityEngine.UIElements;

namespace Animocity.Cities
{
    public class PowerGrid
    {
        public const float COST_LV = 0.01f;
        public const float COST_HV = 0.0001f;

        private Graph<Vector2Int> graph;

        public bool HasPower
        {
            get
            {
                return GetPowerProduced() >= GetPowerUsed();
            }
        }

        private List<BuildingComponent_PowerConsumer> _consumers; 
        public IEnumerable<BuildingComponent_PowerConsumer> Consumers
        {
            get
            {
                return _consumers;
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

        public delegate void PowerGridEvent(PowerGrid grid, List<BuildingComponent_Power> gridBuildingsAffected);

        public event PowerGridEvent PowerConsumersAdded;
        public event PowerGridEvent PowerSuppliersAdded;

        public event PowerGridEvent PowerConsumersRemoved;
        public event PowerGridEvent PowerSuppliersRemoved;

        private List<Vector2Int> _lowVoltage;
        private List<Vector2Int> _highVoltage;

        private  PowerGrid() { }
        public PowerGrid(List<BuildingComponent_PowerSupplier> suppliers, List<BuildingComponent_PowerConsumer> users, List<Vector2Int> lowVoltageTiles, List<Vector2Int> highVoltageTiles) 
        {
            this._suppliers = suppliers;
            this._consumers = users;
            this._lowVoltage = lowVoltageTiles;
            this._highVoltage = highVoltageTiles;

            this.UpdateGridMembership();
            this.UpdateGraphRepresentation();
        }

        private void UpdateGridMembership()
        {
            foreach(var user in _consumers)
            {
                user.UpdateGrid(this);
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

            //this.graph = Graph<Vector2Int>.FromGridSquares(costs);
        }

        public void ConnectToGrid(BuildingComponent_Power buildingToConnect)
        {
            if(buildingToConnect is BuildingComponent_PowerConsumer)
            {
                AddConsumer(buildingToConnect as BuildingComponent_PowerConsumer);
            }
            else if (buildingToConnect is BuildingComponent_PowerSupplier)
            {
                AddSupplier(buildingToConnect as BuildingComponent_PowerSupplier);
            }
        }
        public void RemoveFromGrid(BuildingComponent_Power buildingToConnect)
        {
            if (buildingToConnect is BuildingComponent_PowerConsumer)
            {
                RemoveConsumer(buildingToConnect as BuildingComponent_PowerConsumer);
            }
            else if (buildingToConnect is BuildingComponent_PowerSupplier)
            {
                RemoveSupplier(buildingToConnect as BuildingComponent_PowerSupplier);
            }
        }

        public void AddConsumer(BuildingComponent_PowerConsumer consumer)
        {
            _consumers.Add( consumer );
            PowerConsumersAdded?.Invoke(this, this.Consumers as List<BuildingComponent_Power>);
        }
        public void AddSupplier(BuildingComponent_PowerSupplier supplier)
        {
            _suppliers.Add(supplier);
            PowerSuppliersAdded?.Invoke(this, this.Consumers as List<BuildingComponent_Power>);
        }
        public void RemoveConsumer(BuildingComponent_PowerConsumer consumer)
        {
            _consumers.Remove(consumer);
            PowerConsumersRemoved?.Invoke(this, this.Consumers as List<BuildingComponent_Power>);
        }
        public void RemoveSupplier(BuildingComponent_PowerSupplier supplier)
        {
            _suppliers.Remove(supplier);
            PowerSuppliersRemoved?.Invoke(this, this.Consumers as List<BuildingComponent_Power>);
        }

        public float GetPowerProduced()
        {
            if (Suppliers == null || Suppliers.Count() == 0)
            {
                return 0;
            }
            return Suppliers.Sum((supplier) => supplier.PowerData.powerAmount);
        }
        public float GetPowerUsed()
        {
            if (Consumers == null || Consumers.Count() == 0)
            {
                return 0;
            }
            return Consumers.Sum((cons) => cons.PowerData.powerAmount);
        }

        private void UpdateSupplierLocations()
        {
            this.getSupplierAt.Clear();
            foreach(var supplier in _suppliers)
            {
                this.getSupplierAt.Add(supplier.Building.GridLocation, supplier);
            }
        }




        /*
        public static PowerGrid MergeGrids(PowerGrid[] gridsToMerge)
        {
            List<BuildingComponent_PowerConsumer> newConsumers = new ();
            List<BuildingComponent_PowerSupplier> newSuppliers = new ();
            List<Vector2Int> newLowVoltage = new ();
            List<Vector2Int> newHighVoltage = new ();

            foreach (var grid in gridsToMerge)
            {
                newConsumers.AddRange(grid.Consumers);
                newSuppliers.AddRange(grid.Suppliers);
                newLowVoltage.AddRange(grid._lowVoltage);
                newHighVoltage.AddRange(grid._highVoltage);
            }

            return new PowerGrid(newSuppliers, newConsumers, newLowVoltage, newHighVoltage);
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
        }


        public void Resupply(BuildingComponent_PowerSupplier supplier)
        {
            supplyAvailable[supplier] = supplier.PowerData.powerConsumption;
        }
        */
    }
}

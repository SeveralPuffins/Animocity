using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animocity.Utilities;
using Animocity.Cities.Algorithms;

namespace Animocity.Cities
{
    public class HousingManager
    {

        private List<BuildingComponent_Housing> _houses;

        public IEnumerable<BuildingComponent_Housing> Houses
        { get { return _houses; } }

        public HousingManager(IEnumerable<CityGrid> cityGrids) 
        {
            _houses = new List<BuildingComponent_Housing>();
        }
        public int GetHousingCapacity()
        {
            if(_houses == null || _houses.Count() == 0)
            {
                return 0;
            }

            return _houses.Sum((house) => house.HousingData.capacity);
        }

        public void ResetResidences()
        {
            foreach (var house in this._houses)
            {
                house.ResetResidents();
            }
        }

        public float GetHousingSatisfaction(PopulationBlue population)
        {
            if (_houses == null || _houses.Count() == 0)
            {
                return 0;
            }

            float totalPop = 0;
            float totalSatisfaction = 0;
            foreach(var house in _houses)
            {
                totalPop += house.CurrentResidents(population);
                totalSatisfaction += totalPop * house.CurrentSatisfaction;
            }
            if(totalPop == 0)
            {
                return 0;
            }
            return totalSatisfaction/totalPop;
        }

        public void AddHouse(BuildingComponent_Housing newHouse)
        {
            _houses.Add(newHouse);
        }
        public void RemoveHouse(BuildingComponent_Housing oldHouse)
        {
            _houses.Remove(oldHouse);
        }

        internal bool TryFindAcceptableCommute(CityGrid grid, Vector2Int gridLocation, PopulationBlue pop, int assignedPopMax, out int popsSuccessfullyHoused)
        {
            var gridHouses = _houses.Where(house => house.Building.Grid == grid && house.HousingData.capacity > house.NumTotalResidents);

            if (gridHouses.Count() > 0) {
                
                var endpoints = gridHouses.ToDictionary((house)=>house.Building.GridLocation,house=>house);


                if (TransportManager.Current.TryFindPaths(grid, gridLocation, endpoints.Keys, TransportManager.MAX_COMMUTE_COST, out var paths))
                {
                    popsSuccessfullyHoused = 0;
                    foreach ( var path in paths)
                    {
                        int roomsRequested = assignedPopMax - popsSuccessfullyHoused;
                        if (roomsRequested <= 0) break;

                        var house = endpoints[path.Destination];
                        int roomsAvailable = house.RoomsAvailable;

                        int roomsAssigned = Mathf.Min(roomsAvailable, roomsRequested);
                        house.AddResidents(roomsAssigned, pop);
                        popsSuccessfullyHoused += roomsAssigned;

                        house.AddCommute(new Commute(pop, roomsAssigned, path.GetNodes.Reverse().ToList(), grid));
                    }

                    return true;
                }
                else
                {
                    popsSuccessfullyHoused = 0;
                    return false;
                }
            }
            else 
            {
                popsSuccessfullyHoused = 0;
                return false;
            }
        }

        public Dictionary<PopulationBlue, int> GetHomelessAfterHousingUnemployed()
        {
            var unhousedWorkers = new Dictionary<PopulationBlue, int>(WorkforceManager.Current.unassignedWorkers);
            int unhousedWorkersCount = unhousedWorkers.Values.Sum();


            foreach (var pop in unhousedWorkers.Keys.ToArray())
            {
                if (unhousedWorkers[pop] == 0) continue;

                var availableHousing =
                    this._houses
                        .Where((h) => h.RoomsAvailable > 0)
                        .OrderBy((h) => h.CurrentSatisfaction)
                        .ToList();

                foreach (var house in availableHousing)
                {
                    int maxToHouse = System.Math.Min(unhousedWorkers[pop]   , house.RoomsAvailable);

                    house.AddResidents(maxToHouse, pop);
                    unhousedWorkersCount -= maxToHouse;
                    unhousedWorkers[pop] -= maxToHouse;

                    if (unhousedWorkers[pop] == 0) break;
                }
                if (unhousedWorkersCount == 0) break;
            }

            return unhousedWorkers;
        }
    }
}
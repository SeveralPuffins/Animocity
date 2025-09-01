using System.Linq;
using ZLinq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animocity.Utilities;
using Animocity.Cities.Algorithms;
using BlueprintSystem;

namespace Animocity.Cities
{
    public class HousingManager
    {

        private List<BuildingComponent_Housing> _houses;
        private List<BuildingComponent_NeedSource> _needsBuildings;

        public IEnumerable<BuildingComponent_Housing> Houses
        { get { return _houses; } }

        public HousingManager() 
        {
            _houses = new List<BuildingComponent_Housing>();
            _needsBuildings = new List<BuildingComponent_NeedSource>();
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
        public void AddNeedsBuilding(BuildingComponent_NeedSource newSource)
        {
            _needsBuildings.Add(newSource);
        }
        public void RemoveNeedsBuilding(BuildingComponent_NeedSource oldSource)
        {
            _needsBuildings.Remove(oldSource);
        }

        internal bool TryFindAcceptableCommute(MultiPoint gridLocation, PopulationBlue pop, int assignedPopMax, out int popsSuccessfullyHoused)
        {
            var gridHouses = _houses.AsValueEnumerable().Where(house => house.HousingData.capacity > house.NumTotalResidents);

            if (gridHouses.Count() > 0) {
                
                var endpoints = gridHouses.ToDictionary((house)=>new MultiPoint(house.Building.GridLocation, CityOverview.Current.CityMultiGrid.GetIndex(house.Building.Grid)),house=>house);


                if (TransportManager.Current.TryFindPaths(gridLocation, endpoints.Keys, TransportManager.MAX_COMMUTE_COST, out var paths))
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

                        house.AddCommute(new Commute(pop, roomsAssigned, path.GetNodes.Reverse().ToList()));
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

        private Dictionary<PopulationBlue, int> unhousedWorkers;
        public Dictionary<PopulationBlue, int> GetHomelessAfterHousingUnemployed()
        {
            if (unhousedWorkers == null)
            {
                unhousedWorkers = new(WorkforceManager.Current.unassignedWorkers);
            }
            else
            {
                foreach (var key in WorkforceManager.Current.unassignedWorkers.Keys)
                {
                    unhousedWorkers[key] = WorkforceManager.Current.unassignedWorkers[key];
                }
            }
            int unhousedWorkersCount = unhousedWorkers.Values.AsValueEnumerable().Sum();


            foreach (var pop in unhousedWorkers.Keys.ToArray())
            {
                if (unhousedWorkers[pop] == 0) continue;

                var availableHousing =
                    this._houses.AsValueEnumerable()
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

        public void GetFoodNeedRoutes()
        {
            foreach(var needsBuilding in _needsBuildings)
            {
                needsBuilding.ClearNeedSubscriberResidences();
            }

            var sources = _needsBuildings.ToArray();

            foreach(var house in this._houses) 
            {
                if (house.TryFindBestFoodSource(sources, out var source))
                {
                    source.AddSubscriber(house);
                }
            }
        }

        internal bool FeedTheHomeless(float timeInMinutes)
        {
            var foodPerPersonPerMinute = 0.02f;
            var totalHomeless = CityOverview.Current.Homeless.Values.Sum();

            if(totalHomeless > 0)
            {
                var food = BlueprintDatabase<ResourceBlue>.Fetch("Food");

                float consumption = foodPerPersonPerMinute * timeInMinutes * totalHomeless;

                if (CityOverview.Current.HasResource(food, consumption)) {
                    CityOverview.Current.TakeResource(Vector2Int.zero, food, consumption);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
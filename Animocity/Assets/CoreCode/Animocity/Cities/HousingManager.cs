using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animocity.Utilities;

namespace Animocity.Cities
{
    public class HousingManager
    {
        public const float MAX_COMMUTE_COST = 30.0f;
        private List<BuildingComponent_Housing> _houses;
        private Dictionary<CityGrid, TransportGrid> transportGrids;
        public IEnumerable<BuildingComponent_Housing> Houses
        { get { return _houses; } }

        public HousingManager(IEnumerable<CityGrid> cityGrids) 
        { 
            transportGrids = new();
            foreach (CityGrid cityGrid in cityGrids)
            {
                transportGrids.Add(cityGrid, new TransportGrid());
            }

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

        public float GetHousingSatisfaction(int population)
        {
            if (_houses == null || _houses.Count() == 0)
            {
                return 0;
            }

            _houses.Sort((house1, house2) => house2.CurrentSatisfaction.CompareTo(house1.CurrentSatisfaction));

            int idx = _houses
                        .CumulativeSum((house)=>house.HousingData.capacity)
                        .ToList()
                        .FindIndex((sum)=>sum>=population);

            if(idx == -1)
            {
                float housedSatisfaction =
                      _houses
                        .Sum((house) => house.HousingData.capacity * house.CurrentSatisfaction);
                float capacity =
                        _houses
                        .Sum((house) => house.HousingData.capacity);

                float homelessness = (1f*population - capacity) / (1f * population);

                return Mathf.Max(0,(housedSatisfaction/capacity) * (1f - homelessness) * (1f - homelessness) - 2.0f * homelessness); 
            }
            else // No homelessness
            {
                float totalSatisfaction =
                      _houses
                        .Take(idx)
                        .Sum((house) => house.HousingData.capacity * house.CurrentSatisfaction);
                float capacityOfOccupiedHouses =
                        _houses
                        .Take(idx)
                        .Sum((house) => house.HousingData.capacity);

                return totalSatisfaction/capacityOfOccupiedHouses;
            }
        }

        public void AddHouse(BuildingComponent_Housing newHouse)
        {
            _houses.Add(newHouse);
        }
        public void RemoveHouse(BuildingComponent_Housing oldHouse)
        {
            _houses.Remove(oldHouse);
        }

        public void AddTransport(BuildingComponent_Transport transport, CityGrid grid, Vector2Int location)
        {
            transportGrids[grid].AddSquare(location, transport.TransportData.transitCost);
        }

        public void RemoveTransport(BuildingComponent_Transport transport, CityGrid grid, Vector2Int location)
        {

        }

        internal bool TryFindHousing(CityGrid grid, Vector2Int gridLocation, int assignedPopMax, out int popsSuccessfullyHoused)
        {
            TransportGrid transportGrid = transportGrids[grid];

            var gridHouses = _houses.Where(house => house.Building.Grid == grid && house.HousingData.capacity > house.NumCurrentResidents);

            if (gridHouses.Count() > 0) {
                
                var endpoints = gridHouses.ToDictionary((house)=>house.Building.GridLocation,house=>house);


                if (transportGrid.TryFindPaths(gridLocation, endpoints.Keys, MAX_COMMUTE_COST, out var paths))
                {
                    popsSuccessfullyHoused = 0;
                    foreach ( var path in paths)
                    {
                        int roomsRequested = assignedPopMax - popsSuccessfullyHoused;
                        if (roomsRequested <= 0) break;

                        var house = endpoints[path.Destination];
                        int roomsAvailable = house.RoomsAvailable;

                        int roomsAssigned = Mathf.Min(roomsAvailable, roomsRequested);
                        house.AddResidents(roomsAssigned);
                        popsSuccessfullyHoused += roomsAssigned;
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
    }
}
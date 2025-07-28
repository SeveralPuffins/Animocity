using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animocity.Utilities;

namespace Animocity.Cities
{
    public class HousingManager
    {
        private List<BuildingComponent_Housing> _houses;
        public IEnumerable<BuildingComponent_Housing> Houses
        { get { return _houses; } }

        public HousingManager() 
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
    }
}
using Animocity.Cities.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities
{
    public class TransportManager
    {
        public const float MAX_COMMUTE_COST = 3.0f;

        private Dictionary<CityGrid, TransportGrid> _transportGrids;
        public static TransportManager Current;
        public TransportManager(CityOverview city) 
        {
            _transportGrids = new Dictionary<CityGrid, TransportGrid>();
            foreach(var grid in city.cityGrids)
            {
                _transportGrids.Add(grid, new TransportGrid());
            }
            Current = this;
        }

        public void AddTransport(CityGrid grid, float transitCost, Vector2Int location)
        {
            _transportGrids[grid].AddSquare(location, transitCost);
        }

        public void RemoveTransport(CityGrid grid, Vector2Int location)
        {
            _transportGrids[grid].RemoveSquare(location);
        }


        public bool TryFindPaths(CityGrid grid, Vector2Int startLocation, IEnumerable<Vector2Int> endpoints, float maxDistance, out List<Path<Vector2Int>> paths)
        {
            return _transportGrids[grid].TryFindPaths(startLocation, endpoints, maxDistance, out paths);
        }

        public HashSet<Vector2Int> GetConnectedTiles(CityGrid grid, Vector2Int startLocation, float maxDistance)
        {
            return _transportGrids[grid].GetConnectedTiles(startLocation, maxDistance);
        }
    }
}

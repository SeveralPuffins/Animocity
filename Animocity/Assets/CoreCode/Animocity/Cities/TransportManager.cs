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
        private MultiGrid cityGrid;
        public TransportGrid TransportGrid;
        public static TransportManager Current;
        public TransportManager(CityOverview city) 
        {
            this.cityGrid = city.CityMultiGrid;
            TransportGrid = new TransportGrid(cityGrid);
            Current = this;
        }

        public void AddTransport(CityGrid grid, float transitCost, Vector2Int location)
        {
            var idx = cityGrid.GetIndex(grid);
            var mp = new MultiPoint(location, idx);
            TransportGrid.AddSquare(mp, transitCost);
        }

        public void RemoveTransport(CityGrid grid, Vector2Int location)
        {
            var idx = cityGrid.GetIndex(grid);
            var mp = new MultiPoint(location, idx);
            TransportGrid.RemoveSquare(mp);
        }


        public bool TryFindPaths(MultiPoint startLocation, IEnumerable<MultiPoint> endpoints, float maxDistance, out List<Path<MultiPoint>> paths)
        {
            return TransportGrid.TryFindPaths(startLocation, endpoints, maxDistance, out paths);
        }

        public HashSet<MultiPoint> GetConnectedTiles(MultiPoint startLocation, float maxDistance)
        {
            return TransportGrid.GetConnectedTiles(startLocation, maxDistance);
        }
    }
}

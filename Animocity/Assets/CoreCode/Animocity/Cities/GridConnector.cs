using UnityEngine;

namespace Animocity.Cities
{
    public class GridConnector
    {
        public CityGrid FirstGrid { get; private set; }
        public CityGrid SecondGrid { get; private set; }

        public Vector2Int FirstGridConnectionPoint { get; private set; }
        public Vector2Int SecondGridConnectionPoint { get; private set; }

        public float ConnectionTransitCost { get; private set; }

        public GridConnector(CityGrid firstGrid, CityGrid secondGrid, Vector2Int firstConnectionPoint, Vector2Int secondConnectionPoint, float transitCost)
        {
            this.FirstGrid = firstGrid;
            this.SecondGrid = secondGrid;
            this.FirstGridConnectionPoint = firstConnectionPoint;
            this.SecondGridConnectionPoint = secondConnectionPoint;
            this.ConnectionTransitCost = transitCost;
        }
    }
}
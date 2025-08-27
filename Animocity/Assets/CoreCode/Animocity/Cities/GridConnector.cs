using UnityEngine;

namespace Animocity.Cities
{
    public class GridConnector
    {
        public MultiPoint ConnectionEndA { get; private set; }
        public MultiPoint ConnectionEndB { get; private set; }

        public float ConnectionTransitCost { get; private set; }

        public GridConnector(int firstGrid, int secondGrid, Vector2Int firstConnectionPoint, Vector2Int secondConnectionPoint, float transitCost)
        {
            this.ConnectionEndA = new MultiPoint(firstConnectionPoint, firstGrid);
            this.ConnectionEndB = new MultiPoint(secondConnectionPoint, secondGrid);
            this.ConnectionTransitCost = transitCost;
        }
    }
}
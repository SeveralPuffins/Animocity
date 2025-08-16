using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities.Algorithms
{
    //Used to be generic. Could try that again? May take some design thoguht to do.
    public class Commute
    {
        private List<Vector2Int> route;
        public PopulationBlue PopulationType { get; private set; }
        public int CommuterCount { get; private set; }
        public float TravelCost {  get; private set; }

        private TransportGrid grid;

        public Commute(PopulationBlue populationType, int commuterCount, List<Vector2Int> route, TransportGrid grid)
        {
            this.grid = grid;
            this.route = route;
            this.CommuterCount = commuterCount;
            this.PopulationType = populationType;
        }

        Vector2Int Origin
        {
            get
            {
                return route[0];
            }
        }
        Vector2Int Destination
        {
            get
            {
                return route[route.Count()-1];
            }
        }

        public bool IsValid()
        {
            if (grid.TryCheckRoute(this.route, out float cost))
            {
                this.TravelCost = cost;
                return true;
            }
            return false;
        }
    }
}

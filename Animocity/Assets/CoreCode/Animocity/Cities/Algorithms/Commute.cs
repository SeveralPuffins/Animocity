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
        private List<MultiPoint> route;
        public PopulationBlue PopulationType { get; private set; }
        public int CommuterCount { get; private set; }
        public float TravelCost {  get; private set; }

        public Commute(PopulationBlue populationType, int commuterCount, List<MultiPoint> route)
        {
            this.route = route;
            this.CommuterCount = commuterCount;
            this.PopulationType = populationType;
        }

        public void Reverse()
        {
            MonoBehaviour.print("Reverse Commute!");
            route.Reverse();
        }

        public int Length => route.Count;
        public MultiPoint GetNode(int index)
        {
            if (index < 0) index += Length;
            return route[index%Length];
        }

        public MultiPoint Origin
        {
            get
            {
                return route[0];
            }
        }
        public MultiPoint Destination
        {
            get
            {
                return route[route.Count()-1];
            }
        }

        public bool IsValid()
        {
            if (TransportManager.Current.TransportGrid.TryCheckRoute(this.route, out float cost))
            {
                this.TravelCost = cost;
                return true;
            }
            return false;
        }
    }
}

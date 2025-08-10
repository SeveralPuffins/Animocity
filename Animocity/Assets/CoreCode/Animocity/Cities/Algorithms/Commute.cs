using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities.Algorithms
{
    public class Commute<T>
    {
        private List<T> path;
        public int CommuterCount { get; private set; }
        public float TravelCost {  get; private set; }

        T Origin
        {
            get
            {
                return path[0];
            }
        }
        T Destination
        {
            get
            {
                return path[path.Count-1];
            }
        }

        private Graph<T> graph;

        public bool IsValid()
        {
            if (graph.TryCheckPath(this.path, out float cost))
            {
                this.TravelCost = cost;
                return true;
            }
            return false;
        }
        public void IncrementCommuters(int changeInCommuters)
        {
            this.CommuterCount += changeInCommuters;
        }
    }
}

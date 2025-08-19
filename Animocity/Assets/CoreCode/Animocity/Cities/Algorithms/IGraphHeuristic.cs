using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Cities.Algorithms
{
    public interface IGraphHeuristic<T>
    {
        public float GetHeuristicDistanceBetween(T firstLocation, T SecondLocation);
    }
}

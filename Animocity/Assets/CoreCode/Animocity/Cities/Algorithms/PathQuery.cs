using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.Cities.Algorithms
{
    public class PathQuery<T>
    {
        public float maxCost;
        public HashSet<T> endpoints;
        public T start;

        public PathQuery(T start, IEnumerable<T> endpoints)
        {
            this.endpoints = new HashSet<T>(endpoints);
            this.start = start;
            this.maxCost = float.MaxValue;
        }

        public PathQuery(T start, IEnumerable<T> endpoints, float maxCost)
        {
            this.endpoints = new HashSet<T>(endpoints);
            this.start = start;
            this.maxCost = maxCost;
        }
    }
}

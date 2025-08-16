using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace Animocity.Cities.Algorithms
{
    public class Path<T>
    {
        public int Length { get; private set; }
        private T[] _nodes;
        private float[] _costs;
        public Path(IEnumerable<T> nodes, IEnumerable<float> costs) 
        {
            Contract.Assert(nodes.Count() == costs.Count(), "Nodes and costs were different lengths on this path");
            Contract.Assert(nodes.Count() > 0, "Path contains 0 nodes.");

            _nodes = nodes.ToArray();
            _costs = costs.ToArray();
        }

        public Queue<T> GetNodes => new Queue<T>(_nodes);
        public Queue<float> GetCosts => new Queue<float>(_costs);

        public T Origin => _nodes[0];
        public T Destination => _nodes[_nodes.Length - 1];
        private float? _totalCost;
        public float TotalCost
        {
            get
            {
                if (_totalCost == null)
                {
                    _totalCost = _costs.Sum();
                }
                return _totalCost.Value;
            }
        }
    }
}

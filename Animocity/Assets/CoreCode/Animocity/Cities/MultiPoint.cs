using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Animocity.Cities
{
    public struct MultiPoint
    {
        public Vector2Int Coords { get; private set; }
        public int GridIndex { get; private set; }

        public MultiPoint(Vector2Int coords, int idx)
        {
            this.Coords = coords;
            this.GridIndex = idx;
        }

        public Vector3 ToWorldPoint()
        {
            return CityOverview.Current.CityMultiGrid.GetGrid(GridIndex).WorldFromCell(Coords);
        }


        public override bool Equals(object obj)
        {
            MultiPoint other = (MultiPoint) obj;
            
            return other.GridIndex == this.GridIndex && other.Coords == this.Coords;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Coords, GridIndex);
        }
    }
}
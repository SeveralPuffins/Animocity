using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using System.Diagnostics.Contracts;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace Animocity
{
    public class Polygon
    {
        private Vector2[] _vertices;
        public Rect BoundingBox { get; private set; }
        public Polygon(IEnumerable<Vector2> vertices) 
        {
            Contract.Assert(vertices != null, "Vertices were null in Polygon constructor");
            Contract.Assert(vertices.Count() > 2, "Fewer than 3 vertices supplied, polygon has no inside.");
            this._vertices = vertices.ToArray();
            RecalculateBounds();
        }

        private void RecalculateBounds()
        {
            float xMin, yMin, xMax, yMax;

            xMin = yMin = float.MaxValue;
            xMax = yMax = float.MinValue;

            foreach (Vector2 v in _vertices)
            {
                xMin = Math.Min(xMin, v.x);
                xMax = Math.Max(xMax, v.x);
                yMin = Math.Min(yMin, v.y);
                yMax = Math.Max(yMax, v.y);
            }

            BoundingBox = new Rect(xMin, yMin, xMax-xMin, yMax-yMin);
        }

        public bool Contains(Vector2 point)
        {
            if(!BoundingBox.Contains(point)) return false;

            float startX = BoundingBox.xMin - 1f;
            float length = BoundingBox.xMax + 1f - startX;

            //Count crossings at the y level of th epoint (essentially count ray intersections)
            bool result = false;
            int j = _vertices.Length - 1;
            for (int i = 0; i < _vertices.Length; i++)
            {
                if (_vertices[i].y < point.y && _vertices[j].y >= point.y ||
                    _vertices[j].y < point.y && _vertices[i].y >= point.y)
                {
                    if (_vertices[i].x + (point.y - _vertices[i].y) /
                       (_vertices[j].y - _vertices[i].y) *
                       (_vertices[j].x - _vertices[i].x) < point.x)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}

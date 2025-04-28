using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Animocity.UI
{
    public struct RectHighlight
    {
        public Rect rect;
        public Color clr;

        public RectHighlight(Rect rect, Color clr)
        {
            this.rect = rect;
            this.clr = clr;
        }
    }
}

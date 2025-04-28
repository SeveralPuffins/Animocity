using System;
using UnityEngine;

namespace Animocity.UI
{
    public static class UIUtilities
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}

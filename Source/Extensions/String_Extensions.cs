// String_Extensions.cs
// Copyright Karel Kroeze, 2017-2020

using System.Collections.Generic;
using Verse;

namespace WorkTab {
    public static class String_Extensions {
        private static readonly Dictionary<string, float> _noWrapWidths = new Dictionary<string, float>();

        public static float NoWrapWidth(this string text) {
            if (!_noWrapWidths.TryGetValue(text, out float width)) {
                // store old WW setting
                bool oldWW = Text.WordWrap;

                // set WW off, and calculate width
                Text.WordWrap = false;
                width = Text.CalcSize(text).x;

                // restore WW setting
                Text.WordWrap = oldWW;

                // cache width
                _noWrapWidths.Add(text, width);
            }

            return width;
        }
    }
}

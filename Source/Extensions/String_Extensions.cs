// Karel Kroeze
// String_Extensions.cs
// 2017-06-15

using System.Collections.Generic;
using Verse;

namespace WorkTab
{
    public static class String_Extensions
    {
        private static Dictionary<string, float> _noWrapWidths = new Dictionary<string, float>();

        public static float NoWrapWidth( this string text )
        {
            float width;
            if ( !_noWrapWidths.TryGetValue( text, out width ) )
            {
                // store old WW setting
                var oldWW = Text.WordWrap;

                // set WW off, and calculate width
                Text.WordWrap = false;
                width = Text.CalcSize( text ).x;

                // restore WW setting
                Text.WordWrap = oldWW;

                // cache width
                _noWrapWidths.Add(text, width);
            }

            return width;
        }
    }
}
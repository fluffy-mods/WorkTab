// Karel Kroeze
// LabelUtilities.cs
// 2017-06-15

using UnityEngine;
using Verse;

namespace WorkTab
{
    public static class LabelUtilities
    {
        public static void Label(Rect canvas, string text, Color color, string tip = "")
        {
            Label(canvas, text, color, GameFont.Small, TextAnchor.UpperLeft, tip);
        }

        public static void Label(Rect canvas, string text, GameFont font, string tip = "")
        {
            Label(canvas, text, Color.white, font, TextAnchor.UpperLeft, tip);
        }

        public static void Label(Rect canvas, string text, TextAnchor anchor, string tip = "")
        {
            Label(canvas, text, Color.white, GameFont.Small, anchor, tip);
        }

        public static void Label(Rect canvas, string text, string tip = "")
        {
            Label(canvas, text, Color.white, GameFont.Small, TextAnchor.UpperLeft, tip);
        }

        public static void Label(Rect canvas, string text, Color color, GameFont font, TextAnchor anchor, string tip = "")
        {
            // cache old font settings
            Color oldColor = GUI.color;
            GameFont oldFont = Text.Font;
            TextAnchor oldAnchor = Text.Anchor;

            // set new ones
            GUI.color = color;
            Text.Font = font;
            Text.Anchor = anchor;

            // draw label and tip
            Widgets.Label(canvas, text);
            if (!tip.NullOrEmpty())
                TooltipHandler.TipRegion(canvas, tip);

            // reset settings
            GUI.color = oldColor;
            Text.Font = oldFont;
            Text.Anchor = oldAnchor;
        }

        public static void VerticalLabel(Rect rect, string text, float margin = Constants.Margin)
        {
            // store the scaling matrix
            Matrix4x4 matrix = GUI.matrix;

            // rotate and then apply the scaling
            GUI.matrix = Matrix4x4.identity;
            GUIUtility.RotateAroundPivot(-90f, rect.center);
            GUI.matrix = matrix * GUI.matrix;

            var flipped = new Rect(0f, 0f, rect.height, rect.width) { center = rect.center };
            Widgets.Label(flipped, text);

            // restore the original scaling matrix
            GUI.matrix = matrix;
        }
    }
}
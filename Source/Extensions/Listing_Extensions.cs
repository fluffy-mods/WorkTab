using UnityEngine;
using Verse;

namespace WorkTab
{
    public static class Listing_Extensions
    {
        public static void CheckboxLabeled(this Listing_Standard listing, string label, ref bool checkOn, bool disabled, string tooltip = null)
        {
            Rect rect = listing.GetRect(Text.LineHeight);
            if (!tooltip.NullOrEmpty())
                TooltipHandler.TipRegion(rect, tooltip);
            Widgets.DrawHighlightIfMouseover(rect);
            Widgets.CheckboxLabeled(rect, label, ref checkOn, disabled);
            listing.Gap(listing.verticalSpacing);
        }

        public static void TextFieldNumericLabeled<T>(this Listing_Standard listing, string label, ref T value, string buffer, float min = 0, float max = float.MaxValue, string tooltip = null, float textfieldWidthFraction = 1 / 3f) where T : struct
        {
            Rect rect = listing.GetRect(Text.LineHeight);
            Rect fieldRect = rect;
            Rect labelRect = rect;
            fieldRect.xMin = rect.xMax - rect.width * textfieldWidthFraction;
            labelRect.xMax = fieldRect.xMin;
            if (!tooltip.NullOrEmpty())
                TooltipHandler.TipRegion(rect, tooltip);
            Widgets.DrawHighlightIfMouseover(rect);
            Widgets.Label(labelRect, label);
            Widgets.TextFieldNumeric(fieldRect, ref value, ref buffer, min, max );
            listing.Gap(listing.verticalSpacing);
        }
    }
}
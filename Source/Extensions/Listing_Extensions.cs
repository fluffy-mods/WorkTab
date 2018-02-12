using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            Widgets.TextFieldNumeric(fieldRect, ref value, ref buffer, min, max);
            listing.Gap(listing.verticalSpacing);
        }

        public static void ColorBoxes(this Listing_Standard listing, ref List<string> colors, string tooltip = null)
        {
            Rect rect = listing.GetRect(Text.LineHeight);
            if (!tooltip.NullOrEmpty())
                TooltipHandler.TipRegion(rect, tooltip);
            
            Rect boxRect = rect;
            boxRect.width = Text.LineHeight;
            boxRect.x += Text.LineHeight + 1;
            Rect hexRect = boxRect;
            hexRect.x += boxRect.width + 1;
            hexRect.width = 60; // 60 to fit "FFFFFF"
            for (int i = 0; i < colors.Count; i++)
            {
                if(i == colors.Count - 1)
                {
                    Widgets.TextArea(boxRect, "...", true);
                    boxRect.x += boxRect.width + 1;
                    hexRect.x += boxRect.width + 1;
                }
                string color = colors[i];
                Color c = new Color();
                ColorUtility.TryParseHtmlString("#"+color, out c);
                Widgets.DrawBoxSolid(boxRect, c);
                Widgets.DrawBox(boxRect);
                string newHex = Widgets.TextField(hexRect, color, 6, new Regex("[0-9a-fA-F]*"));
                colors[i] = newHex;
                float xdiff = boxRect.width + 2 + hexRect.width;
                boxRect.x += xdiff;
                hexRect.x += xdiff;
            }

            Rect lessRect = rect;
            lessRect.width = Text.LineHeight;
            Rect moreRect = rect;
            moreRect.width = Text.LineHeight;
            moreRect.x = rect.xMax - Text.LineHeight;

            if (colors.Count > 2)
                if (Widgets.ButtonText(lessRect, "-"))
                    colors.RemoveAt(colors.Count - 2);
            if (colors.Count < 9)
                if (Widgets.ButtonText(moreRect, "+"))
                    colors.Insert(colors.Count - 1, "FFFFFF");

            listing.Gap(listing.verticalSpacing);
        }
    }
}
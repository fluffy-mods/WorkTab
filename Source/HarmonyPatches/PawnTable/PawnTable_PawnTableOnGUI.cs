// PawnTable_PawnTableOnGUI.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using WorkTab.Extensions;

namespace WorkTab {
    [HarmonyPatch(typeof(PawnTable), nameof(PawnTable.PawnTableOnGUI))]
    public class PawnTable_PawnTableOnGUI {
        private static          int       _hoveredColumnContent   = -1;
        private static          int       _hoveredColumnLabel     = -1;
        private static          int       _hoveredRowContent      = -1;
        private static          int       _hoveredRowLabel        = -1;
        private static readonly Type      ptt                     = typeof( PawnTable );
        private static readonly FieldInfo cachedColumnWidthsField = AccessTools.Field( ptt, "cachedColumnWidths" );
        private static readonly FieldInfo cachedRowHeightsField   = AccessTools.Field( ptt, "cachedRowHeights" );


        private static readonly MethodInfo RecacheIfDirtyMethod = AccessTools.Method( ptt, "RecacheIfDirty" );

        private static readonly FieldInfo standardMarginField =
            AccessTools.Field( typeof( Window ), "StandardMargin" );

        static PawnTable_PawnTableOnGUI() {
            if (RecacheIfDirtyMethod == null) {
                throw new NullReferenceException("RecacheIfDirty field not found.");
            }

            if (cachedColumnWidthsField == null) {
                throw new NullReferenceException("cachedColumnWidths field not found.");
            }

            if (cachedRowHeightsField == null) {
                throw new NullReferenceException("cachedRowHeights field not found.");
            }

            if (standardMarginField == null) {
                throw new NullReferenceException("standardMargin field not found.");
            }
        }

        public static bool Prefix(PawnTable __instance,
                                   Vector2 position,
                                   PawnTableDef ___def,
                                   ref Vector2 ___scrollPosition)
        //harmony 1.2.0.1 gives access to private fields by ___name.
        {
            if (___def != PawnTableDefOf.Work) // only apply our changes on the work tab.
{
                return true;
            }

            if (Event.current.type == EventType.Layout) {
                return false;
            }

            _hoveredRowLabel = -1;

            RecacheIfDirtyMethod.Invoke(__instance, null);

            // get fields
            Vector2 cachedSize              = __instance.Size;
            var columns                 = __instance.Columns;
            List<float> cachedColumnWidths      = cachedColumnWidthsField.GetValue( __instance ) as List<float>;
            float cachedHeaderHeight      = __instance.HeaderHeight;
            float cachedHeightNoScrollbar = __instance.HeightNoScrollbar;
            Vector2 headerScrollPosition    = new Vector2( ___scrollPosition.x, 0f );
            Vector2 labelScrollPosition     = new Vector2( 0f, ___scrollPosition.y );
            List<Pawn> cachedPawns             = __instance.PawnsListForReading;
            List<float> cachedRowHeights        = cachedRowHeightsField.GetValue( __instance ) as List<float>;
            float standardWindowMargin    = (float) standardMarginField.GetRawConstantValue();

            // this is the main change, vanilla hardcodes both outRect and viewRect to the cached size.
            // Instead, we want to limit outRect to the available view area, so a horizontal scrollbar can appear.
            float labelWidth = cachedColumnWidths[0];
            var labelCol   = columns[0];
            // ideally this method would be called with a Rect outRect
            // indicating the window it is being drawn in instead
            // of a Vector2 position
            var outRect = __instance.get_OutRect();
            float outWidth   = outRect.width - labelWidth;
            float viewWidth  = cachedSize.x - labelWidth - 16f;

            Rect labelHeaderRect = new Rect(
                position.x,
                position.y,
                labelWidth,
                cachedHeaderHeight );

            Rect headerOutRect = new Rect(
                position.x + labelWidth,
                position.y,
                outWidth,
                cachedHeaderHeight );
            Rect headerViewRect = new Rect(
                0f,
                0f,
                viewWidth,
                cachedHeaderHeight );

            Rect labelOutRect = new Rect(
                position.x,
                position.y + cachedHeaderHeight,
                labelWidth,
                cachedSize.y - cachedHeaderHeight );
            Rect labelViewRect = new Rect(
                0f,
                0f,
                labelWidth,
                cachedHeightNoScrollbar - cachedHeaderHeight );

            Rect tableOutRect = new Rect(
                position.x + labelWidth,
                position.y + cachedHeaderHeight,
                outWidth,
                cachedSize.y - cachedHeaderHeight );
            Rect tableViewRect = new Rect(
                0f,
                0f,
                viewWidth,
                cachedHeightNoScrollbar - cachedHeaderHeight );

            // increase height of table to accomodate scrollbar if necessary and possible.
            if (viewWidth > outWidth && cachedSize.y + 16f < UI.screenHeight) {
                // NOTE: this is probably optimistic about the available height, but it appears to be what vanilla uses.
                tableOutRect.height += 16f;
            }

            // we need to add a scroll area to the column headers to make sure they stay in sync with the rest of the table, but the first (labels) column should be frozen.
            labelCol.Worker.DoHeader(labelHeaderRect, __instance);

            // scroll area for the rest of the columns - HORIZONTAL ONLY
            IntVec3 pos = IntVec3.Zero;
            Widgets.BeginScrollView(headerOutRect, ref headerScrollPosition, headerViewRect, false);
            for (int i = 1; i < columns.Count; i++) {
                int colWidth;
                if (i == columns.Count - 1) {
                    colWidth = (int) (viewWidth - pos.x);
                } else {
                    colWidth = (int) cachedColumnWidths[i];
                }

                Rect rect = new Rect( pos.x, 0f, colWidth, (int) cachedHeaderHeight );

                // column highlight sync
                if (Mouse.IsOver(rect)) {
                    _hoveredColumnLabel = i;
                }

                if (_hoveredColumnContent == i) {
                    Widgets.DrawHighlight(rect);
                }

                columns[i].Worker.DoHeader(rect, __instance);
                pos.x += colWidth;
            }

            _hoveredColumnContent = -1;
            Widgets.EndScrollView();
            ___scrollPosition.x = headerScrollPosition.x;

            // scrollview for label column - VERTICAL ONLY
            if (_hoveredColumnLabel == 0) {
                Widgets.DrawHighlight(labelOutRect);
            }

            Widgets.BeginScrollView(labelOutRect, ref labelScrollPosition, labelViewRect, false);
            Rect labelRect = labelOutRect.AtZero();
            for (int j = 0; j < cachedPawns.Count; j++) {
                labelRect.height = (int) cachedRowHeights[j];

                // only draw if on screen
                if (tableViewRect.height <= tableOutRect.height ||
                     labelRect.y - ___scrollPosition.y + (int) cachedRowHeights[j] >= 0f &&
                     labelRect.y - ___scrollPosition.y <= tableOutRect.height) {
                    GUI.color = new Color(1f, 1f, 1f, 0.2f);
                    Widgets.DrawLineHorizontal(0f, pos.z, tableViewRect.width);
                    GUI.color = Color.white;

                    labelCol.Worker.DoCell(labelRect, cachedPawns[j], __instance);
                    if (_hoveredRowContent == j) {
                        Widgets.DrawHighlight(labelRect);
                    }

                    if (Mouse.IsOver(labelRect)) {
                        _hoveredRowLabel = j;
                    }

                    if (cachedPawns[j].Downed) {
                        GUI.color = new Color(1f, 0f, 0f, 0.5f);
                        Widgets.DrawLineHorizontal(0f, labelRect.center.y, labelWidth);
                        GUI.color = Color.white;
                    }
                }

                labelRect.y += (int) cachedRowHeights[j];
            }

            Widgets.EndScrollView();
            ___scrollPosition.y = labelScrollPosition.y;

            _hoveredRowContent = -1;

            // And finally, draw the rest of the table - SCROLLS VERTICALLY AND HORIZONTALLY
            Widgets.BeginScrollView(tableOutRect, ref ___scrollPosition, tableViewRect);
            pos.x = 0;
            for (int k = 1; k < columns.Count; k++) {
                int columnWidth;
                if (k == columns.Count - 1) {
                    columnWidth = (int) (viewWidth - pos.x);
                } else {
                    columnWidth = (int) cachedColumnWidths[k];
                }

                Rect column = new Rect( pos.x, pos.y, columnWidth, (int) tableOutRect.height );
                if (Mouse.IsOver(column)) {
                    Widgets.DrawHighlight(column);
                    _hoveredColumnContent = k;
                }

                if (_hoveredColumnLabel == k) {
                    Widgets.DrawHighlight(column);
                }

                pos.x += columnWidth;
            }

            _hoveredColumnLabel = -1;
            for (int j = 0; j < cachedPawns.Count; j++) {
                pos.x = 0;
                // only draw if on screen
                if (tableViewRect.height <= tableOutRect.height ||
                     pos.y - ___scrollPosition.y + (int) cachedRowHeights[j] >= 0f &&
                     pos.y - ___scrollPosition.y <= tableOutRect.height) {
                    GUI.color = new Color(1f, 1f, 1f, 0.2f);
                    Widgets.DrawLineHorizontal(0f, pos.y, tableViewRect.width);
                    GUI.color = Color.white;
                    Rect rowRect = new Rect( 0f, pos.y, tableViewRect.width, (int) cachedRowHeights[j] );

                    // synchronize row highlights 
                    if (Mouse.IsOver(rowRect)) {
                        Widgets.DrawHighlight(rowRect);
                        _hoveredRowContent = j;
                    } else if (_hoveredRowLabel == j) {
                        Widgets.DrawHighlight(rowRect);
                    }

                    for (int k = 1; k < columns.Count; k++) {
                        int cellWidth;
                        if (k == columns.Count - 1) {
                            cellWidth = (int) (viewWidth - pos.x);
                        } else {
                            cellWidth = (int) cachedColumnWidths[k];
                        }

                        Rect rect3 = new Rect( pos.x, pos.y, cellWidth, (int) cachedRowHeights[j] );
                        columns[k].Worker.DoCell(rect3, cachedPawns[j], __instance);
                        pos.x += cellWidth;
                    }

                    if (cachedPawns[j].Downed) {
                        GUI.color = new Color(1f, 0f, 0f, 0.5f);
                        Widgets.DrawLineHorizontal(0f, rowRect.center.y, tableViewRect.width);
                        GUI.color = Color.white;
                    }
                }

                pos.y += (int) cachedRowHeights[j];
            }

            Widgets.EndScrollView();

            return false;
        }
    }
}

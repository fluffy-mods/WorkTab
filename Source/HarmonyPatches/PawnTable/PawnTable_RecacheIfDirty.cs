// PawnTable_RecacheIfDirty.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace WorkTab {
    [HarmonyPatch(typeof(PawnTable), "RecacheIfDirty")]
    public class PawnTable_RecacheIfDirty {
        private static readonly FieldInfo cachecColumnWidthsField =
            AccessTools.Field( typeof( PawnTable ), "cachedColumnWidths" );

        private static readonly FieldInfo cachedSizeField = AccessTools.Field( typeof( PawnTable ), "cachedSize" );
        private static readonly FieldInfo dirtyField      = AccessTools.Field( typeof( PawnTable ), "dirty" );

        static PawnTable_RecacheIfDirty() {
            if (dirtyField == null) {
                throw new NullReferenceException("PawnTable.dirty field not found.");
            }

            if (cachedSizeField == null) {
                throw new NullReferenceException("PawnTable.cachedSize field not found.");
            }

            if (cachecColumnWidthsField == null) {
                throw new NullReferenceException("PawnTable.cachecColumnWidths field not found.");
            }
        }

        private static void Postfix(PawnTable __instance, bool __state) {
            // cop out if cache was not dirty.
            if (!__state) {
                return;
            }

            Logger.Debug("Checking minimum column widths...");

            // loop over columns to check that they satisfy their minimum width.
            List<float> columnWidths      = cachecColumnWidthsField.GetValue( __instance ) as List<float>;
            var columns           = __instance.Columns;
            bool anyColumnAdjusted = false;
            float tableWidth        = 0f;
            for (int i = 0; i < columns.Count; i++) {
                var minWidth = columns[i].Worker.GetMinWidth( __instance );
                if (columnWidths[i] < minWidth) {
                    columnWidths[i] = minWidth;
                    anyColumnAdjusted = true;
                }

                tableWidth += columnWidths[i];
            }

            // If any columns were adjusted, also adjust the table size.
            if (anyColumnAdjusted) {
                Vector2 size = new Vector2( tableWidth, __instance.Size.y );
                if (cachedSizeField == null) {
                    throw new NullReferenceException("PawnTable.cachedSize not found.");
                }

                cachedSizeField.SetValue(__instance, size);
            }
        }

        private static void Prefix(PawnTable __instance, ref bool __state, PawnTableDef ___def) {
            __state = (bool) dirtyField.GetValue(__instance) && ___def == PawnTableDefOf.Work;
            if (__state) {
                Logger.Debug("ColumnWidths dirty");
            }
        }
    }
}

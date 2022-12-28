using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorkTab.Extensions {
    internal static class PawnTable_Extensions {
        private static ConditionalWeakTable<PawnTable, StrongBox<Rect>> outRectDictionary=new ConditionalWeakTable<PawnTable, StrongBox<Rect>>();
        /// <summary>
        /// Sets the rectangle the <see cref="PawnTable"/> is drawn in.
        /// </summary>
        /// <param name="pawnTable">The <see cref="PawnTable"/> being extended.</param>
        /// <param name="outRect">The rectangle the <see cref="PawnTable"/> will be drawn in.</param>
        internal static void set_OutRect(this PawnTable pawnTable, Rect outRect) {
            var value = outRectDictionary.GetValue(
                pawnTable,
                a => new StrongBox<Rect>(outRect)
            );
            value.Value = outRect;
        }
        /// <summary>
        /// Gets the rectangle the <see cref="PawnTable"/> will be drawn in.
        /// </summary>
        /// <param name="pawnTable">The <see cref="PawnTable"/> being extended.</param>
        /// <returns>The rectangle the <see cref="PawnTable"/> will be drawn in.</returns>
        internal static Rect get_OutRect(this PawnTable pawnTable) {
            return outRectDictionary.GetOrCreateValue(pawnTable).Value;
        }
    }
}

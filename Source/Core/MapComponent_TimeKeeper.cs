// MapComponent_TimeKeeper.cs
// Copyright Karel Kroeze, 2018-2020

using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab {
    public class MapComponent_TimeKeeper: MapComponent {
        private int currentHour = -1;

        public MapComponent_TimeKeeper(Map map) : base(map) {
        }

#if DEBUG
        public override void MapComponentOnGUI() {
            base.MapComponentOnGUI();
            Rect statusRect = new Rect( 6f, UI.screenHeight / 2f, 150f, 50f );
            Widgets.Label(statusRect, typeof(MainTabWindow_WorkTab).Assembly.GetName().Version.ToString());
        }
#endif

        public override void MapComponentTick() {
            base.MapComponentTick();

            // check if an hour passed every second, staggering out maps
            if ((Find.TickManager.TicksGame + GetHashCode()) % 60 == 0
              && GenLocalDate.HourOfDay(map) != currentHour) {
                // update our current hour
                currentHour = GenLocalDate.HourOfDay(map);

                Logger.Debug("forcing priority refresh for " + currentHour.FormatHour());

                // make pawns update their priorities
                foreach (Pawn pawn in map.mapPawns.FreeColonistsSpawned) {
                    pawn.workSettings.Notify_UseWorkPrioritiesChanged();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Fluffy_Tabs
{
    public class MapComponent_Timekeeper : MapComponent
    {

        private int _hourOfDay;
        public MapComponent_Timekeeper( Map map ) : base( map ) {}

        #region Overrides of MapComponent

        /// <summary>
        /// The whole point of this mapcomponent is to have a map dependent timekeeping mechanism that makes pawns obey their hourly schedules.
        /// </summary>
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if ( GenLocalDate.HourOfDay( map ) == _hourOfDay )
                return;

            _hourOfDay = GenLocalDate.HourOfDay( map );
            foreach ( Pawn pawn in map.mapPawns.FreeColonistsSpawned )
                pawn.workSettings.Notify_UseWorkPrioritiesChanged();
        }

        #endregion
    }
}

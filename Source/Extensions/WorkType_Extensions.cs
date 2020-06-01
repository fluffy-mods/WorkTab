// WorkType_Extensions.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.Sound;

namespace WorkTab
{
    public static class WorkType_Extensions
    {
        private static readonly Dictionary<WorkTypeDef, List<WorkGiverDef>> _workgiversByType =
            new Dictionary<WorkTypeDef, List<WorkGiverDef>>();

        private static readonly Dictionary<WorkTypeDef, string> workListCache = new Dictionary<WorkTypeDef, string>();

        public static void DecrementPriority( this WorkTypeDef worktype, Pawn pawn, int hour = -1,
                                              List<int> hours = null, bool playSound = true )
        {
            // get default hour if not specified
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            // get and decrement priority
            var priority = pawn.GetPriority( worktype, hour );
            pawn.SetPriority( worktype, priority - 1, hours );

            // play sounds
            if ( Settings.playSounds && playSound && priority > 1 )
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            if ( Settings.playSounds && playSound && priority == 1 )
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
        }

        public static void DecrementPriority( this WorkTypeDef worktype, List<Pawn> pawns, int hour = -1,
                                              List<int> hours = null, bool playSound = true )
        {
            // bail out on bad input
            if ( pawns.NullOrEmpty() )
                return;

            // get default hour if not specified, we're assuming all pawns are on the same map/tile
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawns.FirstOrDefault() );

            // play sounds
            if ( Settings.playSounds && playSound && pawns.Any( p => p.GetPriority( worktype, hour ) != 1 ) )
                SoundDefOf.Tick_High.PlayOneShotOnCamera();

            // decrease priorities that are not 1 only (no wrapping around once we're at max priority)
            foreach ( var pawn in pawns.Where( p => p.GetPriority( worktype, hour ) != 1 ).DistinctTrackers() )
                DecrementPriority( worktype, pawn, hour, hours, false );
        }

        public static void IncrementPriority( this WorkTypeDef worktype, Pawn pawn, int hour = -1,
                                              List<int> hours = null, bool playSound = true )
        {
            // get default hour if not specified
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            // get and increment priority
            var priority = pawn.GetPriority( worktype, hour );
            pawn.SetPriority( worktype, priority + 1, hours );

            // play sounds
            if ( Settings.playSounds && playSound && priority == 0 )
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            if ( Settings.playSounds && playSound && priority > 0 )
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
        }

        public static void IncrementPriority( this WorkTypeDef worktype, List<Pawn> pawns, int hour = -1,
                                              List<int> hours = null, bool playSound = true )
        {
            // bail out on bad input
            if ( pawns.NullOrEmpty() )
                return;

            // get default hour if not specified, we're assuming all pawns are on the same map/tile
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawns.FirstOrDefault() );

            // play sounds
            if ( Settings.playSounds && playSound && pawns.Any( p => p.GetPriority( worktype, hour ) > 0 ) )
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();

            // increase priorities that are > 0 only (no wrapping around once we're at min priority
            foreach ( var pawn in pawns.Where( p => p.GetPriority( worktype, hour ) > 0 ).DistinctTrackers() )
                IncrementPriority( worktype, pawn, hour, hours, false );
        }

        public static string SpecificWorkListString( this WorkTypeDef def )
        {
            string tip;
            if ( workListCache.TryGetValue( def, out tip ) )
                return tip;

            var stringBuilder = new StringBuilder();
            for ( var i = 0; i < def.workGiversByPriority.Count; i++ )
            {
                stringBuilder.Append( def.workGiversByPriority[i].LabelCap );
                if ( def.workGiversByPriority[i].emergency )
                    stringBuilder.Append( " (" + "EmergencyWorkMarker".Translate() + ")" );
                if ( i < def.workGiversByPriority.Count - 1 ) stringBuilder.AppendLine();
            }

            tip = stringBuilder.ToString();
            workListCache.Add( def, tip );
            return tip;
        }

        public static List<WorkGiverDef> WorkGivers( this WorkTypeDef worktype )
        {
            List<WorkGiverDef> result;
            if ( !_workgiversByType.TryGetValue( worktype, out result ) )
            {
                result = DefDatabase<WorkGiverDef>
                        .AllDefsListForReading.Where( wg => wg.workType == worktype ).ToList();
                _workgiversByType[worktype] = result;
            }

            return result;
        }
    }
}
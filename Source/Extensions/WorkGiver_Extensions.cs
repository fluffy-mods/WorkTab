// WorkGiver_Extensions.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Multiplayer.API;
using Verse.Sound;

namespace WorkTab
{
    public static class WorkGiver_Extensions
    {
        public static void DecrementPriority( this WorkGiverDef workgiver, Pawn pawn, int hour,
                                              List<int> hours, bool playSound = true )
        {
            // get default hour if not specified
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            // get and decrement priority
            var priority = pawn.GetPriority( workgiver, hour );
            pawn.SetPriority( workgiver, priority - 1, hours );

            // play sounds
            if ( Settings.playSounds && playSound && priority > 1 )
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            if ( Settings.playSounds && playSound && priority == 1 )
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
        }

        [SyncMethod]
        public static void DecrementPriority( this WorkGiverDef workgiver, List<Pawn> pawns, int hour,
                                              List<int> hours, bool playSound = true )
        {
            // bail out on bad input
            if ( pawns.NullOrEmpty() )
                return;

            // get default hour if not specified, we're assuming all pawns are on the same map/tile
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawns.FirstOrDefault() );

            // play sounds
            if ( Settings.playSounds && playSound && pawns.Any( p => p.GetPriority( workgiver, hour ) != 1 ) )
                SoundDefOf.Tick_High.PlayOneShotOnCamera();

            // decrease priorities that are not 1 only (no wrapping around once we're at max priority)
            foreach ( var pawn in pawns.Where( p => p.GetPriority( workgiver, hour ) != 1 ).DistinctTrackers() )
                DecrementPriority( workgiver, pawn, hour, hours, false );
        }

        public static void IncrementPriority( this WorkGiverDef workgiver, Pawn pawn, int hour,
                                              List<int> hours, bool playSound = true )
        {
            // get default hour if not specified
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            // get and increment priority
            var priority = pawn.GetPriority( workgiver, hour );
            pawn.SetPriority( workgiver, priority + 1, hours );

            // play sounds
            if ( Settings.playSounds && playSound && priority == 0 )
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            if ( Settings.playSounds && playSound && priority > 0 )
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
        }

        [SyncMethod]
        public static void IncrementPriority( this WorkGiverDef workgiver, List<Pawn> pawns, int hour,
                                              List<int> hours, bool playSound = true )
        {
            // bail out on bad input
            if ( pawns.NullOrEmpty() )
                return;

            // get default hour if not specified, we're assuming all pawns are on the same map/tile
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawns.FirstOrDefault() );

            // play sounds
            if ( Settings.playSounds && playSound && pawns.Any( p => p.GetPriority( workgiver, hour ) > 0 ) )
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();

            // increase priorities that are > 0 only (no wrapping around once we're at min priority
            foreach ( var pawn in pawns.Where( p => p.GetPriority( workgiver, hour ) > 0 ).DistinctTrackers() )
                IncrementPriority( workgiver, pawn, hour, hours, false );
        }

        internal static IEnumerable<Pawn> DistinctTrackers( this IEnumerable<Pawn> pawns )
        {
            var knownTrackers = new HashSet<PriorityTracker>();
            foreach ( var pawn in pawns )
                if ( knownTrackers.Add( PriorityManager.Get[pawn] ) )
                    yield return pawn;
        }
    }
}
// Karel Kroeze
// WorkGiver_Extensions.cs
// 2017-05-28

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
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
            int priority = pawn.GetPriority( workgiver, hour );
            pawn.SetPriority( workgiver, priority - 1, hours );

            // play sounds
            if ( Settings.playSounds && playSound && priority > 1 )
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
            if ( Settings.playSounds && playSound && priority == 1 )
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
        }

        public static void IncrementPriority( this WorkGiverDef workgiver, Pawn pawn, int hour,
                                              List<int> hours, bool playSound = true )
        {
            // get default hour if not specified
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            // get and increment priority
            int priority = pawn.GetPriority( workgiver, hour );
            pawn.SetPriority( workgiver, priority + 1, hours );

            // play sounds
            if ( Settings.playSounds && playSound && priority == 0 )
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();
            if ( Settings.playSounds && playSound && priority > 0 )
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();
        }

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
                SoundDefOf.AmountIncrement.PlayOneShotOnCamera();

            // decrease priorities that are not 1 only (no wrapping around once we're at max priority)
            foreach ( Pawn pawn in pawns.Where( p => p.GetPriority( workgiver, hour ) != 1 ) )
                DecrementPriority( workgiver, pawn, hour, hours, false );
        }

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
                SoundDefOf.AmountDecrement.PlayOneShotOnCamera();

            // increase priorities that are > 0 only (no wrapping around once we're at min priority
            foreach ( Pawn pawn in pawns.Where( p => p.GetPriority( workgiver, hour ) > 0 ) )
                IncrementPriority( workgiver, pawn, hour, hours, false );
        }
    }
}
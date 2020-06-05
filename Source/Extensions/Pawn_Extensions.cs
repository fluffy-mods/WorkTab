// Pawn_Extensions.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using RimWorld;
using Multiplayer.API;
using Verse;

namespace WorkTab
{
    public static class Pawn_Extensions
    {
        public static bool AllowedToDo( this Pawn pawn, WorkGiverDef workgiver )
        {
            if ( pawn?.story == null )
                return true;
            return !pawn.WorkTypeIsDisabled( workgiver.workType )                                   &&
                   ( pawn.CombinedDisabledWorkTags & workgiver.workTags )          == WorkTags.None &&
                   ( pawn.CombinedDisabledWorkTags & workgiver.workType.workTags ) == WorkTags.None;
        }

        public static bool CapableOf( this Pawn pawn, WorkGiverDef workgiver )
        {
            return !workgiver.requiredCapacities.Any( c => !pawn.health.capacities.CapableOf( c ) );
        }

        public static void DisableAll( this Pawn pawn )
        {
            foreach ( var worktype in DefDatabase<WorkTypeDef>.AllDefsListForReading )
                pawn.SetPriority( worktype, 0, null );
        }

        public static int[] GetPriorities( this Pawn pawn, WorkTypeDef worktype )
        {
            return PriorityManager.Get[pawn].GetPriorities( worktype );
        }

        public static int[] GetPriorities( this Pawn pawn, WorkGiverDef workgiver )
        {
            return PriorityManager.Get[pawn][workgiver].Priorities;
        }

        public static int GetPriority( this Pawn pawn, WorkTypeDef worktype, int hour )
        {
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            return PriorityManager.Get[pawn].GetPriority( worktype, hour );
        }

        public static int GetPriority( this Pawn pawn, WorkGiverDef workgiver, int hour )
        {
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            return PriorityManager.Get[pawn].GetPriority( workgiver, hour );
        }

        [SyncMethod]
        public static void SetPriority( this Pawn pawn, WorkTypeDef worktype, int priority, List<int> hours )
        {
            PriorityManager.Get[pawn].SetPriority( worktype, priority, hours );
        }

        [SyncMethod]
        public static void SetPriority( Pawn pawn, WorkTypeDef worktype, int priority, int hour, bool recache = true )
        {
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            PriorityManager.Get[pawn].SetPriority( worktype, priority, hour, recache );
        }

        [SyncMethod]
        public static void SetPriority( this Pawn pawn, WorkGiverDef workgiver, int priority, List<int> hours )
        {
            PriorityManager.Get[pawn].SetPriority( workgiver, priority, hours );
        }

        [SyncMethod]
        public static void SetPriority( this Pawn pawn, WorkGiverDef workgiver, int priority, int hour,
                                        bool recache = true )
        {
            if ( hour < 0 )
                hour = GenLocalDate.HourOfDay( pawn );

            PriorityManager.Get[pawn].SetPriority( workgiver, priority, hour, recache );
        }
    }
}
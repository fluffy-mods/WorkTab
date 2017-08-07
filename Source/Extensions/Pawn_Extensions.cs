// Karel Kroeze
// Pawn_Extensions.cs
// 2017-05-22

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using static WorkTab.TimeUtilities;

namespace WorkTab
{
    public static class Pawn_Extensions
    {
        public static int GetPriority( this Pawn pawn, WorkTypeDef worktype, int hour )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            // get priorities for all workgivers in worktype
            var priorities = worktype.WorkGivers().Select( wg => GetPriority( pawn, wg, hour ) ).Where( p => p > 0 );

            // if there are no active priorities, return zero
            if ( !priorities.Any() )
                return 0;

            // otherwise, return the lowest number (highest priority).
            return priorities.Min();
        }

        public static int[] GetPriorities(this Pawn pawn, WorkTypeDef worktype)
        {
            int[] priorities = new int[GenDate.HoursPerDay];
            for (int hour = 0; hour < GenDate.HoursPerDay; hour++)
                priorities[hour] = pawn.GetPriority(worktype, hour);
            return priorities;
        }

        public static int GetPriority(this Pawn pawn, WorkGiverDef workgiver, int hour )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            Logger.Trace($"Getting {pawn.LabelShort}'s {workgiver.defName} priority for {hour}");
            return PriorityManager.Get[pawn][workgiver][hour];
        }

        public static int[] GetPriorities(this Pawn pawn, WorkGiverDef workgiver)
        {
            return PriorityManager.Get[pawn][workgiver].Priorities;
        }

        public static void SetPriority( this Pawn pawn, WorkTypeDef worktype, int priority, List<int> hours )
        {
            foreach (int hour in (hours ?? WholeDay))
                SetPriority(pawn, worktype, priority, hour, false);

            PriorityManager.Get[pawn].Recache(worktype);
        }

        public static void SetPriority(Pawn pawn, WorkTypeDef worktype, int priority, int hour, bool recache = true )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            foreach (WorkGiverDef workgiver in worktype.WorkGivers())
                SetPriority(pawn, workgiver, priority, hour, false);

            if (recache)
                PriorityManager.Get[pawn].Recache(worktype);
        }

        public static void SetPriority( this Pawn pawn, WorkGiverDef workgiver, int priority, List<int> hours )
        {
            foreach (int hour in (hours ?? WholeDay))
                SetPriority(pawn, workgiver, priority, hour, false);

            PriorityManager.Get[pawn].Recache(workgiver);
        }

        public static void SetPriority(Pawn pawn, WorkGiverDef workgiver, int priority, int hour, bool recache = true )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);
            if ( priority > Settings.maxPriority )
                priority = 0;
            if ( priority < 0 )
                priority = Settings.maxPriority;

            Logger.Trace( $"Setting {pawn.LabelShort}'s {workgiver.defName} priority for {hour} to {priority}"  );
            PriorityManager.Set[pawn][workgiver][hour] = priority;
            
            if (recache)
                PriorityManager.Get[pawn].Recache(workgiver);
        }

        public static bool CapableOf( this Pawn pawn, WorkGiverDef workgiver )
        {
            return !workgiver.requiredCapacities.Any( c => !pawn.health.capacities.CapableOf( c ) );
        }
    }
}
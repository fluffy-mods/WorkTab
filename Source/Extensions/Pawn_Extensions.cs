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
        public static int GetMinPriority(this Pawn pawn, WorkTypeDef worktype, int hour)
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            // get priorities for all workgivers in worktype
            var priorities = worktype.WorkGivers().Select( wg => GetPriority( pawn, wg, hour ) ).Where( p => p > 0 );

            // if there are no active priorities, return zero
            if ( !priorities.Any() )
                return 0;

            // otherwise, return the lowest number (highest priority).
            if (Find.PlaySettings.useWorkPriorities)
                return priorities.Min();

            // or, in simple mode, just 3.
            return 3;
        }
        public static int GetMaxPriority(this Pawn pawn, WorkTypeDef worktype, int hour)
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            // get priorities for all workgivers in worktype
            var priorities = worktype.WorkGivers().Select( wg => GetPriority( pawn, wg, hour ) ).Where( p => p > 0 );

            // if there are no active priorities, return zero
            if ( !priorities.Any() )
                return 0;

            // otherwise, return the highest number (lowest priority).
            if (Find.PlaySettings.useWorkPriorities)
                return priorities.Max();

            // or, in simple mode, just 3.
            return 3;
        }
        public static bool AnyGiverMissingPriority(this Pawn pawn, WorkTypeDef worktype, int hour)
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            // get priorities for all workgivers in worktype
            return worktype.WorkGivers().Any(wg => GetPriority(pawn, wg, hour) == 0);
        }
        public static int GetPriority( this Pawn pawn, WorkTypeDef worktype, int hour )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            // get priorities for all workgivers in worktype
            var priorities = worktype.WorkGivers().Select( wg => GetPriority( pawn, wg, hour ) );

            // if there are no active priorities, return zero
            if ( !priorities.Any() )
                return 0;

            // otherwise, return the most common number
            if (Find.PlaySettings.useWorkPriorities)
            {
                //count each priority level, track highest
                Dictionary<int, int> priorityCount = new Dictionary<int, int>();
                int highestCount = 0;
                int commonPriority = 0;

                foreach(var p in priorities)
                {
                    int count = 1;
                    if (priorityCount.ContainsKey(p))
                        count = priorityCount[p] + 1;
                    priorityCount[p] = count;

                    if (count > highestCount)
                    {
                        highestCount = count;
                        commonPriority = p;
                    }
                }

                return commonPriority;
            }

            // or, in simple mode, just 3.
            return 3;
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
            if (Find.PlaySettings.useWorkPriorities)
                return PriorityManager.Get[pawn][workgiver][hour];
            
            // or in simple mode, 3 or 0
            return PriorityManager.Get[pawn][workgiver][hour] > 0 ? 3 : 0;
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

        public static void SetPriority( this Pawn pawn, WorkGiverDef workgiver, int priority, int hour, bool recache = true )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);
            if ( priority > Settings.Get().maxPriority )
                priority = 0;
            if ( priority < 0 )
                priority = Settings.Get().maxPriority;

            Logger.Trace( $"Setting {pawn.LabelShort}'s {workgiver.defName} priority for {hour} to {priority}"  );
            PriorityManager.Set[pawn][workgiver][hour] = priority;
            
            if (recache)
                PriorityManager.Get[pawn].Recache(workgiver);
        }

        public static void ChangePriority( this Pawn pawn, WorkTypeDef worktype, int diff, List<int> hours )
        {
            foreach (int hour in (hours ?? WholeDay))
                ChangePriority(pawn, worktype, diff, hour, false);

            PriorityManager.Get[pawn].Recache(worktype);
        }

        public static void ChangePriority(Pawn pawn, WorkTypeDef worktype, int diff, int hour, bool recache = true )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            foreach (WorkGiverDef workgiver in worktype.WorkGivers())
                ChangePriority(pawn, workgiver, diff, hour, false);

            if (recache)
                PriorityManager.Get[pawn].Recache(worktype);
        }

        public static void ChangePriority( this Pawn pawn, WorkGiverDef workgiver, int diff, int hour, bool recache = true )
        {
            if (hour < 0)
                hour = GenLocalDate.HourOfDay(pawn);

            int priority = pawn.GetPriority(workgiver, hour) + diff;
            SetPriority(pawn, workgiver, priority, hour, recache);
        }

        public static bool CapableOf( this Pawn pawn, WorkGiverDef workgiver )
        {
            return !workgiver.requiredCapacities.Any( c => !pawn.health.capacities.CapableOf( c ) );
        }
    }
}
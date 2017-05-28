using System;
using System.Linq;
using RimWorld;
using Verse;

namespace WorkTab {
    public class WorkPriorityTracker: IExposable {
        private WorkGiverDef workgiver;
        private int[] priorities;
        protected internal Pawn pawn;

        public WorkPriorityTracker(){ 
            // Scribe
        }
        
        public WorkPriorityTracker( Pawn pawn, WorkGiverDef workgiver )
        {
            this.pawn = pawn;
            this.workgiver = workgiver;
            this.priorities = new int[GenDate.HoursPerDay];
            var priority = pawn.GetVanillaPriority(workgiver.workType);
            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ ){
                priorities[hour] = priority;
            }
        }

        public int this[int hour]{
            get{
                return priorities[hour];
            }
            set{
                // update priority
                priorities[hour] = value;

                // make pawn update its priorities
                pawn.workSettings.Notify_UseWorkPrioritiesChanged();
            }
        }

        public WorkGiverDef Workgiver => workgiver;

        public void ExposeData()
        {
            Scribe_Defs.Look( ref workgiver, "Workgiver" );

            if ( Scribe.mode == LoadSaveMode.Saving ){
                var _priorities = String.Join( "", priorities.Select( i => i.ToString() ).ToArray() );
                Scribe_Values.Look( ref _priorities, "Priorities" );
            }
            if (Scribe.mode == LoadSaveMode.LoadingVars ){
                string _priorities = "";
                Scribe_Values.Look( ref _priorities, "Priorities" );
                priorities = _priorities.ToArray().Select( c => int.Parse( c.ToString() ) ).ToArray();
            }
        }
    }
}
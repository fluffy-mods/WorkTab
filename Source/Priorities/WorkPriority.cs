using System;
using System.Linq;
using RimWorld;
using Verse;

namespace WorkTab {
    public class WorkPriority: IExposable {
        private WorkGiverDef workgiver;
        private int[] priorities;
        protected internal PriorityTracker _parent;
        
        // Scribe
        public WorkPriority( PriorityTracker parent ){ 
            _parent = parent;
        }
        
        public WorkPriority( PriorityTracker parent, WorkGiverDef workgiver ): this( parent )
        {
            this.workgiver = workgiver;
            this.priorities = new int[GenDate.HoursPerDay];
            var priority = parent.Pawn?.GetVanillaPriority(workgiver.workType) ?? 0;
            Logger.Debug($"Initiating worktracker for {parent.Pawn.LabelShort}, Priority: {priority}, default: {Settings.defaultPriority}");
            if ( priority > 0 ) priority = Settings.defaultPriority; 
            for ( int hour = 0; hour < GenDate.HoursPerDay; hour++ ){
                priorities[hour] = priority;
            }
        }

        public WorkPriority Clone( PriorityTracker newParent )
        {
            var clone = new WorkPriority( newParent )
            {
                priorities = (int[])priorities.Clone(),
                workgiver = workgiver
            };
            return clone;
        }

        public int this[int hour]{
            get{
                return priorities[hour];
            }
            set
            {
                // check if we're allowed to do this job
                if (value > 0 && !_parent.Pawn.AllowedToDo( workgiver ) )
                {
                    Logger.Debug($"Tried to set priority for {workgiver.label} to {_parent.Pawn?.LabelShort}, who is incapable of that work.");
                    return;
                }

                // update priority
                priorities[hour] = value;

                // make pawn update its priorities
                _parent.Pawn?.workSettings.Notify_UseWorkPrioritiesChanged();
            }
        }

        public WorkGiverDef Workgiver => workgiver;

        public bool EverAssigned => priorities.Any(p => p > 0);

        public int[] Priorities => priorities;

        public void ExposeData()
        {
            try
            {
                Scribe_Defs.Look( ref workgiver, "Workgiver" );
            }
            catch ( Exception e )
            {
                Log.Warning(
                    "WorkTab :: failed to load priorities. Did you disable a mod? If so, this message can safely be ignored." +
                    e.Message + "\n\n" + e.StackTrace );
            }

            if ( Scribe.mode == LoadSaveMode.Saving ){
                var _priorities = string.Join( "", priorities.Select( i => i.ToString() ).ToArray() );
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
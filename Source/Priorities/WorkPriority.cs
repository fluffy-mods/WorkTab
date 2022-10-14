// WorkPriority.cs
// Copyright Karel Kroeze, 2018-2020

using System;
using System.Linq;
using RimWorld;
using Verse;

namespace WorkTab {
    public class WorkPriority: IExposable {
        protected internal PriorityTracker _parent;
        private            WorkGiverDef    workgiver;

        // Scribe
        public WorkPriority(PriorityTracker parent) {
            _parent = parent;
        }

        public WorkPriority(PriorityTracker parent, WorkGiverDef workgiver) : this(parent) {
            this.workgiver = workgiver;
            Priorities = new int[GenDate.HoursPerDay];
            int priority = parent.Pawn?.GetVanillaPriority( workgiver.workType ) ?? 0;
            Logger.Debug(
                $"Initiating worktracker for {parent.Pawn.LabelShort}, Priority: {priority}, default: {Settings.defaultPriority}");
            if (priority > 0) {
                priority = Settings.defaultPriority;
            }

            for (int hour = 0; hour < GenDate.HoursPerDay; hour++) {
                Priorities[hour] = priority;
            }
        }

        public bool EverAssigned => Priorities.Any(p => p > 0);

        public int this[int hour] {
            get => Priorities[hour];
            set {
                // check if we're allowed to do this job
                if (value > 0 && !_parent.Pawn.AllowedToDo(workgiver)) {
                    Logger.Debug(
                        $"Tried to set priority for {workgiver.label} to {_parent.Pawn?.LabelShort}, who is incapable of that work.");
                    return;
                }

                // update priority
                Priorities[hour] = value;

                // make pawn update its priorities
                _parent.Pawn?.workSettings.Notify_UseWorkPrioritiesChanged();
            }
        }

        public int[] Priorities { get; private set; }

        public WorkGiverDef Workgiver => workgiver;

        public void ExposeData() {
            try {
                Scribe_Defs.Look(ref workgiver, "Workgiver");
            } catch (Exception e) {
                Log.Warning(
                    "WorkTab :: failed to load priorities. Did you disable a mod? If so, this message can safely be ignored." +
                    e.Message +
                    "\n\n" +
                    e.StackTrace);
            }

            if (Scribe.mode == LoadSaveMode.Saving) {
                string _priorities = string.Join( "", Priorities.Select( i => i.ToString() ).ToArray() );
                Scribe_Values.Look(ref _priorities, "Priorities");
            }

            if (Scribe.mode == LoadSaveMode.LoadingVars) {
                string _priorities = "";
                Scribe_Values.Look(ref _priorities, "Priorities");
                Priorities = _priorities.ToArray().Select(c => int.Parse(c.ToString())).ToArray();
            }
        }

        public WorkPriority Clone(PriorityTracker newParent) {
            WorkPriority clone = new WorkPriority( newParent )
            {
                Priorities = (int[]) Priorities.Clone(),
                workgiver  = workgiver
            };
            return clone;
        }
    }
}

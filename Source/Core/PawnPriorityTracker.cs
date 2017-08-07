// Karel Kroeze
// PriorityTracker.cs
// 2017-05-22

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using RimWorld;
using Verse;

namespace WorkTab
{
    // todo; implement IExposable
    public class PawnPriorityTracker: IExposable
    {
        private Pawn pawn;
        private Dictionary<WorkGiverDef, WorkPriorityTracker> priorities = new Dictionary<WorkGiverDef, WorkPriorityTracker>();
        private List<WorkPriorityTracker> workPriorityTrackersScribe;

        // caches for ever/partially scheduled
        private Dictionary<WorkGiverDef, bool> _everScheduledWorkGiver = new Dictionary<WorkGiverDef, bool>();
        private Dictionary<WorkGiverDef, bool> _timeScheduledWorkGiver = new Dictionary<WorkGiverDef, bool>();
        private Dictionary<WorkGiverDef, string> _timeScheduledWorkGiverTip = new Dictionary<WorkGiverDef, string>();
        private Dictionary<WorkTypeDef, bool> _everScheduledWorkType = new Dictionary<WorkTypeDef, bool>();
        private Dictionary<WorkTypeDef, bool> _timeScheduledWorkType = new Dictionary<WorkTypeDef, bool>();
        private Dictionary<WorkTypeDef, string> _timeScheduledWorkTypeTip = new Dictionary<WorkTypeDef, string>();
        private Dictionary<WorkTypeDef, bool> _partScheduledWorkType = new Dictionary<WorkTypeDef, bool>();

        // accessors
        public bool EverScheduled(WorkGiverDef workgiver)
        {
            if (!_everScheduledWorkGiver.ContainsKey(workgiver))
                Recache(workgiver);
            return _everScheduledWorkGiver[workgiver];
        }

        public bool TimeScheduled(WorkGiverDef workgiver)
        {
            if (!_timeScheduledWorkGiver.ContainsKey(workgiver))
                Recache(workgiver);
            return _timeScheduledWorkGiver[workgiver];
        }

        public string TimeScheduledTip(WorkGiverDef workgiver)
        {
            if (!_timeScheduledWorkGiverTip.ContainsKey(workgiver))
                Recache(workgiver);
            return _timeScheduledWorkGiverTip[workgiver];
        }

        public bool EverScheduled(WorkTypeDef worktype)
        {
            if (!_everScheduledWorkType.ContainsKey(worktype))
                Recache(worktype);
            return _everScheduledWorkType[worktype];
        }

        public bool TimeScheduled(WorkTypeDef worktype)
        {
            if (!_timeScheduledWorkType.ContainsKey(worktype))
                Recache(worktype);
            return _timeScheduledWorkType[worktype];
        }

        public string TimeScheduledTip(WorkTypeDef worktype)
        {
            if (!_timeScheduledWorkTypeTip.ContainsKey(worktype))
                Recache(worktype);
            return _timeScheduledWorkTypeTip[worktype];
        }

        public bool PartScheduled(WorkTypeDef worktype)
        {
            if (!_partScheduledWorkType.ContainsKey(worktype))
                Recache(worktype);
            return _partScheduledWorkType[worktype];
        }

        public void Recache(WorkGiverDef workgiver, bool bubble = true)
        {
            // recache workgiver stuff
            var priorities = pawn.GetPriorities(workgiver);
            _everScheduledWorkGiver[workgiver] = priorities.Any(p => p > 0);
            _timeScheduledWorkGiver[workgiver] = priorities.Distinct().Count() > 1;
            _timeScheduledWorkGiverTip[workgiver] = WorkUtilities.TimeScheduledTip(pawn, priorities, workgiver.label);

            // also recache worktype
            if (bubble)
                Recache(workgiver.workType, false);
        }

        public void Recache(WorkTypeDef worktype, bool bubble = true)
        {
            var workgivers = worktype.WorkGivers();
            var priorities = pawn.GetPriorities(worktype);

            // first update all the workgivers (if bubbling down, or not set yet)
            foreach (var workgiver in workgivers)
                if (bubble || !_everScheduledWorkGiver.ContainsKey(workgiver)) // using _everScheduled as a proxy - assumes all these are cached at the same time!
                    Recache(workgiver, false);

            // recache worktype stuff
            _everScheduledWorkType[worktype] = workgivers.Any(wg => _everScheduledWorkGiver[wg]);
            _timeScheduledWorkType[worktype] = workgivers.Any(wg => _timeScheduledWorkGiver[wg]);
            _timeScheduledWorkTypeTip[worktype] = WorkUtilities.TimeScheduledTip(pawn, priorities, worktype.gerundLabel);
            _partScheduledWorkType[worktype] = _everScheduledWorkType[worktype] && workgivers.Any(wg => pawn.GetPriorities(wg).All(p => p == 0));
        }

        public WorkPriorityTracker this[ WorkGiverDef index ]
        {
            get
            {
                if ( !priorities.ContainsKey( index ) )
                {
                    Logger.Debug( $"requested {index.defName} priorities for {pawn.LabelShort}, which did not yet exist." );
                    priorities.Add( index, new WorkPriorityTracker( pawn, index ) );
                }
                return priorities[index];
            }
            set => priorities[index] = value;
        }

        public PawnPriorityTracker(){
            // Scribe
        }

        public PawnPriorityTracker( Pawn pawn )
        {
            this.pawn = pawn;
            foreach ( WorkGiverDef workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading )
                priorities.Add( workgiver, new WorkPriorityTracker( pawn, workgiver ) );
        }

        public void ExposeData()
        {
            Scribe_References.Look( ref pawn, "Pawn" );
            
            if ( Scribe.mode == LoadSaveMode.Saving ){
                workPriorityTrackersScribe = priorities.Values.ToList();
                Scribe_Collections.Look( ref workPriorityTrackersScribe, "Priorities", LookMode.Deep );
            }
            if ( Scribe.mode == LoadSaveMode.LoadingVars ){
                Scribe_Collections.Look( ref workPriorityTrackersScribe, "Priorities", LookMode.Deep );
            }
            if ( Scribe.mode == LoadSaveMode.PostLoadInit ){
                priorities = workPriorityTrackersScribe
                    // check if any workgivers were removed midgame (don't try this at home, kids!)
                    .Where( k => k.Workgiver != null )
                    // reinstate the dictionary
                    .ToDictionary( k => k.Workgiver );

                // set pawn value in trackers
                foreach ( KeyValuePair<WorkGiverDef, WorkPriorityTracker> tracker in priorities )
                    tracker.Value.pawn = pawn;
            }
        }
    }
}
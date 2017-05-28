// Karel Kroeze
// PriorityTracker.cs
// 2017-05-22

using System;
using System.Collections.Generic;
using System.Linq;
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
            set { priorities[index] = value; }
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
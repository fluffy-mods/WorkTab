// Karel Kroeze
// PriorityManager.cs
// 2017-05-22

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace WorkTab
{
    public class PriorityManager: GameComponent
    {
        public Dictionary<Pawn, PawnPriorityTracker> priorities = new Dictionary<Pawn, PawnPriorityTracker>();

        private bool _showScheduler;
        public bool ShowScheduler
        {
            get { return _showScheduler; }
            set
            {
                if ( value == _showScheduler )
                    return;

                _showScheduler = value;
                MainTabWindow_WorkTab.Instance.RecacheTimeBarRect();
            }
        }

        public bool ShowPriorities
        {
            get { return Find.PlaySettings.useWorkPriorities; }
            set
            {
                if ( value == Find.PlaySettings.useWorkPriorities )
                    return;

                // update setting
                Find.PlaySettings.useWorkPriorities = value;

                // force re-cache of all pawns
                foreach (var pawn in priorities.Keys.ToList())
                    pawn.workSettings.Notify_UseWorkPrioritiesChanged();   
            }
        }
        private List<Pawn> pawnsScribe;
        private List<PawnPriorityTracker> pawnPriorityTrackersScribe;
        private static PriorityManager _instance;

        public static PriorityManager Get
        {
            get
            {
                if ( _instance == null )
                {
                    throw new NullReferenceException( "Accessing PriorityManager before it was constructed." );
                }
                return _instance;
            }
        }

        public static PriorityManager Set => Get;

        public PawnPriorityTracker this[Pawn pawn]
        {
            get
            {
                PawnPriorityTracker tracker;
                if ( !priorities.TryGetValue( pawn, out tracker ) )
                {
                    tracker = new PawnPriorityTracker( pawn );
                    priorities.Add( pawn, tracker );
                }
                return tracker;
            }
        }

        public PriorityManager( Game game ) : this() {}
        public PriorityManager() { _instance = this; }

        public override void ExposeData(){
            base.ExposeData();

            // purge null pawn elements, note that this also neatly keeps track of periodic garbage collection on autosaves
            var pawns = priorities.Keys.ToList();
            foreach( Pawn pawn in pawns )
                if ( pawn?.Destroyed ?? true ) // null or destroyed
                    priorities.Remove( pawn );

            Scribe_Collections.Look( ref priorities, "Priorities", LookMode.Reference, LookMode.Deep, ref pawnsScribe, ref pawnPriorityTrackersScribe );
            Scribe_Values.Look( ref _showScheduler, "ShowScheduler", false );
        }
        
    }
}
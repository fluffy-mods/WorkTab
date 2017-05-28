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
    // todo; implement IExposable
    public class PriorityManager: GameComponent
    {
        public Dictionary<Pawn, PawnPriorityTracker> priorities = new Dictionary<Pawn, PawnPriorityTracker>();
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
                if ( pawn?.Destroyed ?? true )
                    priorities.Remove( pawn );

            Scribe_Collections.Look( ref priorities, "Priorities", LookMode.Reference, LookMode.Deep, ref pawnsScribe, ref pawnPriorityTrackersScribe );
        }
        
    }
}
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
    public class PawnPriorityTracker: PriorityTracker
    {
        private Pawn pawn;
        
        public PawnPriorityTracker(){
            // Scribe
        }

        public PawnPriorityTracker( Pawn pawn )
        {
            this.pawn = pawn;
            foreach ( WorkGiverDef workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading )
                priorities.Add( workgiver, new WorkPriority( this, workgiver ) );
        }

        public override Pawn Pawn => pawn;

        public override void ExposeData()
        {
            Scribe_References.Look( ref pawn, "Pawn" );
            base.ExposeData();
        }
    }
}
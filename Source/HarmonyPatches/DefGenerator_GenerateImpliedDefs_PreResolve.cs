// Karel Kroeze
// DefGenerator_GenerateImpliedDefs_PreResolve.cs
// 2017-05-22

using Harmony;
using RimWorld;
using Verse;

namespace WorkTab
{
    [HarmonyPatch(typeof( DefGenerator), "GenerateImpliedDefs_PreResolve")]
    public class DefGenerator_GenerateImpliedDefs_PreResolve
    {
        static void Postfix()
        {
            // replace worker on Work MainButtonDef
            DefDatabase<MainButtonDef>.GetNamed( "Work" ).tabWindowClass = typeof( MainTabWindow_WorkTab );

            // insert mood and job columns after name
            var workTable = PawnTableDefOf.Work;
            var index = workTable.columns.IndexOf( PawnColumnDefOf.Label );
            workTable.columns.Insert( index + 1, PawnColumnDefOf.Job );
            workTable.columns.Insert( index + 2, PawnColumnDefOf.Mood );

            // go over PawnColumnDefs and replace all PawnColumnWorker_WorkPriority
            foreach ( PawnColumnDef column in DefDatabase<PawnColumnDef>.AllDefs )
                if ( column.workerClass == typeof( PawnColumnWorker_WorkPriority ) )
                    column.workerClass = typeof( PawnColumnWorker_WorkType );

            // add PawnColumnDefs for all workgivers
            foreach ( WorkGiverDef workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading )
            {
                // prepare the def, note that we're not assigning label or tip, we'll get those from the def later.
                // we're also not adding the def to the table, we'll do that dynamically when a worktype is expanded.
                PawnColumnDef_WorkGiver column = new PawnColumnDef_WorkGiver();
                column.defName = "WorkGiver_" + workgiver.defName;
                column.workgiver = workgiver;
                column.workerClass = typeof( PawnColumnWorker_WorkGiver );
                column.sortable = true;

                // finalize
                column.PostLoad();
                DefDatabase<PawnColumnDef>.Add( column );
            }
        }
    }
}
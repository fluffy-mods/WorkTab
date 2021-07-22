// Copyright Karel Kroeze, 2020-2021.
// WorkTab/WorkTab/DefGenerator_GenerateImpliedDefs_PreResolve.cs

using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WorkTab
{
    [HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
    public class DefGenerator_GenerateImpliedDefs_PreResolve
    {
        private static void Postfix()
        {
            // replace worker on Work MainButtonDef
            MainButtonDefOf.Work.tabWindowClass = typeof(MainTabWindow_WorkTab);

            // get our table
            var workTable = PawnTableDefOf.Work;

            // replace label column
            var labelIndex = workTable.columns.IndexOf(PawnColumnDefOf.Label);
            workTable.columns.RemoveAt(labelIndex);
            workTable.columns.Insert(labelIndex, PawnColumnDefOf.WorkTabLabel);

            // insert mood and job columns before first work column name
            var firstWorkindex =
                workTable.columns.FindIndex(d => d.workerClass == typeof(PawnColumnWorker_WorkPriority));
            workTable.columns.Insert(firstWorkindex, PawnColumnDefOf.Job);
            workTable.columns.Insert(firstWorkindex + 1, PawnColumnDefOf.Mood);

            // go over PawnColumnDefs and replace all PawnColumnWorker_WorkPriority
            foreach (var column in DefDatabase<PawnColumnDef>.AllDefs)
            {
                if (column.workerClass == typeof(PawnColumnWorker_WorkPriority))
                {
                    column.workerClass = typeof(PawnColumnWorker_WorkType);
                }
            }

            // add PawnColumnDefs for all workgivers
            foreach (var workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading)
            {
                // prepare the def, note that we're not assigning label or tip, we'll get those from the def later.
                // we're also not adding the def to the table, we'll do that dynamically when a worktype is expanded.
                var column = new PawnColumnDef_WorkGiver();
                column.defName     = "WorkGiver_" + workgiver.defName;
                column.workgiver   = workgiver;
                column.workerClass = typeof(PawnColumnWorker_WorkGiver);
                column.sortable    = true;

                // finalize
                column.PostLoad();
                DefDatabase<PawnColumnDef>.Add(column);
            }

            // replace and move copy/paste to the right
            var copyPasteColumnIndex = workTable.columns.IndexOf(PawnColumnDefOf.CopyPasteWorkPriorities);
            workTable.columns.RemoveAt(copyPasteColumnIndex);
            // Note; the far right column is a spacer to take all remaining available space, so index should be count - 2 (count - 1 before insert).
            workTable.columns.Insert(workTable.columns.Count - 1,
                                     PawnColumnDefOf.CopyPasteDetailedWorkPriorities);

            // add favourite column before copy paste
            workTable.columns.Insert(workTable.columns.Count - 2, PawnColumnDefOf.Favourite);

            // store this list of all columns
            Controller.allColumns = new List<PawnColumnDef>(workTable.columns);
        }
    }
}
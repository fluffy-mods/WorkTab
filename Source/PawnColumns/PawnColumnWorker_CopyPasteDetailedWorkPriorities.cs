using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WorkTab
{
    public class PawnColumnWorker_CopyPasteDetailedWorkPriorities : PawnColumnWorker_CopyPasteWorkPriorities
    {
        private static Dictionary<WorkGiverDef, int[]> clipboard;
        protected override bool AnythingInClipboard => clipboard != null;

        protected override void CopyFrom(Pawn pawn)
        {
            if (clipboard == null)
                clipboard = new Dictionary<WorkGiverDef, int[]>();

            foreach (var workgiver in DefDatabase<WorkGiverDef>.AllDefsListForReading)
                clipboard[workgiver] = pawn.GetPriorities(workgiver);
        }

        protected override void PasteTo(Pawn pawn)
        {
            foreach (var worktype in DefDatabase<WorkTypeDef>.AllDefsListForReading)
            {
                // do not even try setting priorities for disabled work types.
                if (pawn.WorkTypeIsDisabled(worktype))
                    continue;

                // apply all workgivers for this type
                foreach (var workgiver in worktype.WorkGivers())
                    for (int hour = 0; hour < GenDate.HoursPerDay; hour++)
                        pawn.SetPriority(workgiver, clipboard[workgiver][hour], hour);

                // recache the type (bubbles down to workgivers).
                PriorityManager.Get[pawn].InvalidateCache( worktype );
            }
        }
    }
}
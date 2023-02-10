// Copyright Karel Kroeze, 2020-2021.
// WorkTab/WorkTab/PawnColumnWorker_WorkTabLabel.cs

using RimWorld;
using Verse;
using static WorkTab.Constants;

namespace WorkTab {
    public class PawnColumnWorker_JobText : PawnColumnWorker_Text {
        public override int GetMinWidth(PawnTable table)
        {
            return JobTextWidth;
        }
        private string GetJobString(Pawn pawn)
        {
            return pawn.jobs?.curDriver?.GetReport() ?? "";
        }
        protected override string GetTextFor(Pawn pawn) {
            return GetJobString(pawn);
        }
        protected override string GetTip(Pawn pawn) {
            return GetJobString(pawn);
        }
        public string GetValueToCompare(Pawn pawn)
        {
            return GetJobString(pawn);
        }
        public override int Compare(Pawn a, Pawn b)
        {
            return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
        }
    }
}

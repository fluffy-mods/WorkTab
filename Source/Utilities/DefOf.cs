// Copyright Karel Kroeze, 2020-2021.
// WorkTab/WorkTab/DefOf.cs

using RimWorld;
using Verse;

namespace WorkTab {
    [DefOf]
    public static class PawnColumnDefOf {
        public static                      PawnColumnDef CopyPasteDetailedWorkPriorities;
        public static                      PawnColumnDef CopyPasteWorkPriorities;
        public static                      PawnColumnDef Faction;
        public static                      PawnColumnDef Favourite;
        [MayRequireIdeology] public static PawnColumnDef Guest;
        [MayRequireIdeology] public static PawnColumnDef Ideo;
        public static                      PawnColumnDef Job;
        public static                      PawnColumnDef JobText;
        public static                      PawnColumnDef LabelShortWithIcon;
        public static                      PawnColumnDef Mood;
        public static                      PawnColumnDef WorkTabLabel;
    }

    [DefOf]
    public static class JobDefOf2 {
        public static JobDef BuildSnowman;
        public static JobDef GoForWalk;
        public static JobDef Meditate;
        public static JobDef Play_Billiards;
        public static JobDef Play_Chess;
        public static JobDef Play_Horseshoes;
        public static JobDef Pray;
        public static JobDef Skygaze;
        public static JobDef UseItem;
        public static JobDef UseTelescope;
        public static JobDef ViewArt;
        public static JobDef VisitGrave;
        public static JobDef WatchTelevision;
    }

    [DefOf]
    public static class MainButtonDefOf {
        public static MainButtonDef Work;
    }
}

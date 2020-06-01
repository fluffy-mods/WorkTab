// DefOf.cs
// Copyright Karel Kroeze, 2018-2020

using RimWorld;
using Verse;

namespace WorkTab
{
    [DefOf]
    public static class PawnColumnDefOf
    {
        public static PawnColumnDef Job;
        public static PawnColumnDef Label;
        public static PawnColumnDef Mood;
        public static PawnColumnDef WorkTabLabel;
    }

    [DefOf]
    public static class JobDefOf2
    {
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
}
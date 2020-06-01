// Controller.cs
// Copyright Karel Kroeze, 2020-2020

using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public class Controller : Mod
    {
        public static List<PawnColumnDef> allColumns;

        public Controller( ModContentPack content ) : base( content )
        {
            // initialize settings
            GetSettings<Settings>();

            // prefix implied PawnColumnWorker_WorkType generation 
            // prefix get/set workPriorities
#if DEBUG
            Harmony.DEBUG = true;
#endif
            var harmony = new Harmony( "fluffy.worktab" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );

//            if ( MP.enabled )
//                MP.RegisterAll();
        }

        public override void DoSettingsWindowContents( Rect inRect )
        {
            base.DoSettingsWindowContents( inRect );
            Settings.DoWindowContents( inRect );
        }

        public override string SettingsCategory()
        {
            return "WorkTab".Translate();
        }
    }
}
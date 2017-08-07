// Karel Kroeze
// Controller.cs
// 2017-05-22

using System.Reflection;
using Harmony;
using UnityEngine;
using Verse;

namespace WorkTab
{
    public class Mod: Verse.Mod
    {
        public Mod( ModContentPack content ) : base( content ) {
            // initialize settings
            GetSettings<Settings>();

            // prefix implied PawnColumnWorker_WorkType generation 
            // prefix get/set workPriorities
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            var harmony = HarmonyInstance.Create( "fluffy.worktab" );
            harmony.PatchAll( Assembly.GetExecutingAssembly() );
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "WorkTab".Translate();
        }
    }
}
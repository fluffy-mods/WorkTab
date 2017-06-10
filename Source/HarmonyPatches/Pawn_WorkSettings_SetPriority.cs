// Karel Kroeze
// Pawn_WorkSettings_SetPriority.cs
// 2017-05-22

using Harmony;
using RimWorld;
using Verse;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings), "SetPriority" )]
    public class Pawn_WorkSettings_SetPriority
    {
        static void Prefix( Pawn_WorkSettings __instance, WorkTypeDef w, int priority )
        {
            __instance.Pawn().SetPriority( w, priority, null );
        }
    }
}
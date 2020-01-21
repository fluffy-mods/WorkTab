// Karel Kroeze
// Pawn_WorkSettings_SetPriority.cs
// 2017-05-22

using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings), "SetPriority" )]
    public class Pawn_WorkSettings_SetPriority
    {
        static void Prefix( Pawn_WorkSettings __instance, WorkTypeDef w, ref int priority )
        {
            __instance.Pawn().SetPriority( w, priority, null );

            // TODO: find a more elegant way to stop RW complaining about bad priorities.
            priority = Mathf.Min( priority, 4 );
        }
    }
}
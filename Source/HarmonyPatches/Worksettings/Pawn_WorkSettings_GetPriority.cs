// Pawn_WorkSettings_GetPriority.cs
// Copyright Karel Kroeze, 2020-2020

using HarmonyLib;
using RimWorld;
using Verse;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings ), "GetPriority" )]
    public class Pawn_WorkSettings_GetPriority
    {
        private static bool Prefix( WorkTypeDef w, Pawn_WorkSettings __instance, ref int __result )
        {
            __result = __instance.Pawn().GetPriority( w, -1 );
            return false;
        }
    }
}
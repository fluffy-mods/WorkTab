// Karel Kroeze
// Pawn_WorkSettings_GetPriority.cs
// 2017-05-22

using System;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings), "GetPriority" )]
    public class Pawn_WorkSettings_GetPriority
    {
        static bool Prefix( WorkTypeDef w, Pawn_WorkSettings __instance, ref int __result )
        {
            __result = __instance.Pawn().GetPriority( w );
            return false;
        }
    }
}
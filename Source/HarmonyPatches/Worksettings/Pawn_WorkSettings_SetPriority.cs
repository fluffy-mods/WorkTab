// Pawn_WorkSettings_SetPriority.cs
// Copyright Karel Kroeze, 2020-2020

using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings ), "SetPriority" )]
    public class Pawn_WorkSettings_SetPriority
    {
        private static void Prefix( Pawn_WorkSettings __instance, WorkTypeDef w, ref int priority )
        {
            __instance.Pawn().SetPriority( w, priority, null );

            // TODO: find a more elegant way to stop RW complaining about bad priorities.
            priority = Mathf.Min( priority, 4 );
        }
    }
}
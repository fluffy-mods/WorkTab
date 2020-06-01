// Pawn_WorkSettings_DisableAll.cs
// Copyright Karel Kroeze, 2020-2020

using HarmonyLib;
using RimWorld;

namespace WorkTab
{
    [HarmonyPatch( typeof( Pawn_WorkSettings ), "DisableAll" )]
    public class Pawn_WorkSettings_DisableAll
    {
        public static void Prefix( Pawn_WorkSettings __instance )
        {
            __instance.Pawn().DisableAll();
        }
    }
}